/*
 * $Id$
 * $URL$
 * $Rev$
 * $Author$
 * $Date$
 *
 * smuxi - Smart MUltipleXed Irc
 *
 * Copyright (c) 2005-2006 Mirco Bauer <meebey@meebey.net>
 *
 * Full GPL License: <http://www.gnu.org/licenses/gpl.txt>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307 USA
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Meebey.Smuxi.Common;

namespace Meebey.Smuxi.Engine
{
    public delegate void SimpleDelegate(); 
    
    public class FrontendManager : PermanentRemoteObject, IFrontendUI
    {
#if LOG4NET
        private static readonly log4net.ILog _Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#endif
        private int              _Version = 0;
        private Queue            _Queue  = Queue.Synchronized(new Queue());
        private Thread           _Thread;
        private Session          _Session;
        private IFrontendUI      _UI;
        private ChatModel        _CurrentChat;
        private INetworkManager  _CurrentNetworkManager;
        private bool             _IsFrontendDisconnecting;
        private SimpleDelegate   _ConfigChangedDelegate;
        private bool             _IsFrontendSynced;
        private IList<ChatModel> _SyncedChats = new List<ChatModel>();
        
        public int Version {
            get {
                return _Version;
            }
        }
        
        public SimpleDelegate ConfigChangedDelegate {
            set {
                _ConfigChangedDelegate = value;
            }
        }
        
        public ChatModel CurrentChat {
            get {
                return _CurrentChat;
            }
            set {
                _CurrentChat = value;
            }
        }
        
        public INetworkManager CurrentNetworkManager {
            get {
                return _CurrentNetworkManager;
            }
            set {
                _CurrentNetworkManager = value;
            }
        }
        
        public bool IsFrontendDisconnecting {
            get {
                return _IsFrontendDisconnecting;
            }
            set {
                _IsFrontendDisconnecting = value;
            }
        }
        
        public FrontendManager(Session session, IFrontendUI ui)
        {
            Trace.Call(session, ui);
            
            _Session = session;
            _UI = ui;
            _Thread = new Thread(new ThreadStart(_Worker));
            _Thread.IsBackground = true;
            _Thread.Start();
            
            // register event for config invalidation
            // BUG: when the frontend disconnects there are dangling methods registered!
            //_Session.Config.Changed += new EventHandler(_OnConfigChanged);
            
            // BUG: Session adds stuff to the queue but the frontend is not ready yet!
            // The frontend must Sync() _first_!
            // HACK: so this bug doesn't happen for now
            // actually there is no other way, the frontend must tell us when he is ready to sync!
            //Sync();
        }
        
        public void Sync()
        {
            Trace.Call();
            
            // sync pages            
            foreach (ChatModel chat in _Session.Chats) {
                _AddChat(chat);
            }
            
            // sync current network manager (if any exists)
            if (_Session.NetworkManagers.Count > 0) {
                INetworkManager nm = (INetworkManager)_Session.NetworkManagers[0];
                CurrentNetworkManager = nm;
            }
            
            // sync current page
            _CurrentChat = (ChatModel)_Session.Chats[0];
            
            // sync content of pages
            foreach (ChatModel chat in _Session.Chats) {
                _SyncChat(chat);
            }
            
            _IsFrontendSynced = true;
        }
        
        public void AddSyncedChat(ChatModel chatModel)
        {
            Trace.Call(chatModel);
            
            _SyncedChats.Add(chatModel);
        }
        
        public void NextNetworkManager()
        {
            if (!(_Session.NetworkManagers.Count > 0)) {
                CurrentNetworkManager = null;
            } else {
                int pos = _Session.NetworkManagers.IndexOf(CurrentNetworkManager);
                if (pos < _Session.NetworkManagers.Count - 1) {
                    pos++;
                } else {
                    pos = 0;
                }
                CurrentNetworkManager = (INetworkManager)_Session.NetworkManagers[pos];
            }
            UpdateNetworkStatus();
        }
        
        public void UpdateNetworkStatus()
        {
            if (CurrentNetworkManager != null) {
                SetNetworkStatus(CurrentNetworkManager.ToString());
            } else {
                SetNetworkStatus(String.Format("({0})", _("no network connections")));
            }
        }
        
        public void AddChat(ChatModel chat)
        {
            if (!_SyncedChats.Contains(chat) && _IsFrontendSynced) {
                _AddChat(chat);
            }
        }
        
        private void _AddChat(ChatModel chat)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.AddChat, chat));
        }
        
        public void AddTextToChat(ChatModel chat, string text)
        {
            AddMessageToChat(chat, new MessageModel(text));
        }
        
        public void AddTextToCurrentChat(string text)
        {
            AddTextToChat(CurrentChat, text);
        }
        
        public void EnableChat(ChatModel chat)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.EnableChat, chat));
        }
        
        public void DisableChat(ChatModel chat)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.DisableChat, chat));
        }
        
        public void AddMessageToChat(ChatModel chat, MessageModel msg)
        {
            if (_SyncedChats.Contains(chat)) {
                _AddMessageToChat(chat, msg);
            }
        }
        
        private void _AddMessageToChat(ChatModel chat, MessageModel msg)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.AddMessageToChat, chat, msg));
        }
        
        public void AddMessageToCurrentChat(MessageModel msg)
        {
            AddMessageToChat(CurrentChat, msg);
        }
        
        public void RemoveChat(ChatModel chat)
        {
            if (_SyncedChats.Contains(chat)) {
                _RemoveChat(chat);
            }
        }
        
        private void _RemoveChat(ChatModel chat)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.RemoveChat, chat));
        }
        
        public void SyncChat(ChatModel chat)
        {
             if (!_SyncedChats.Contains(chat) && _IsFrontendSynced) {
                _SyncChat(chat);
            }
        }
        
        private void _SyncChat(ChatModel chat)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.SyncChat, chat));
        }
        
        public void AddPersonToGroupChat(GroupChatModel groupChat, PersonModel person)
        {
            if (_SyncedChats.Contains(groupChat)) {
                _AddPersonToGroupChat(groupChat, person);
            }
        }
        
        private void _AddPersonToGroupChat(GroupChatModel groupChat, PersonModel person)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.AddPersonToGroupChat, groupChat, person));
        }
        
        public void UpdatePersonInGroupChat(GroupChatModel groupChat, PersonModel oldPerson, PersonModel newPerson)
        {
            if (_SyncedChats.Contains(groupChat)) {
                _UpdatePersonInGroupChat(groupChat, oldPerson, newPerson);
            }
        }
        
        private void _UpdatePersonInGroupChat(GroupChatModel groupChat, PersonModel oldPerson, PersonModel newPerson)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.UpdatePersonInGroupChat, groupChat, oldPerson, newPerson));
        }
    
        public void UpdateTopicInGroupChat(GroupChatModel groupChat, string topic)
        {
            if (_SyncedChats.Contains(groupChat)) {
                _UpdateTopicInGroupChat(groupChat, topic);
            }
        }
        
        private void _UpdateTopicInGroupChat(GroupChatModel groupChat, string topic)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.UpdateTopicInGroupChat, groupChat, topic));
        }
    
        public void RemovePersonFromGroupChat(GroupChatModel groupChat, PersonModel person)
        {
            if (_SyncedChats.Contains(groupChat)) {
                _RemovePersonFromGroupChat(groupChat, person);
            }
        }
        
        private void _RemovePersonFromGroupChat(GroupChatModel groupChat, PersonModel person)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.RemovePersonFromGroupChat, groupChat, person));
        }
        
        public void SetNetworkStatus(string status)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.SetNetworkStatus, status));
        }
        
        public void SetStatus(string status)
        {
            _Queue.Enqueue(new UICommandContainer(UICommand.SetStatus, status));
        }
        
        private void _Worker()
        {
            while (true) {
                if (_Queue.Count > 0) {
                    try {
                        UICommandContainer com = (UICommandContainer)_Queue.Dequeue();
                        switch (com.Command) {
                            case UICommand.AddChat:
                                _UI.AddChat((ChatModel)com.Parameters[0]);
                                break;
                            case UICommand.RemoveChat:
                                _UI.RemoveChat((ChatModel)com.Parameters[0]);
                                break;
                            case UICommand.EnableChat:
                                _UI.EnableChat((ChatModel)com.Parameters[0]);
                                break;
                            case UICommand.DisableChat:
                                _UI.DisableChat((ChatModel)com.Parameters[0]);
                                break;
                            case UICommand.SyncChat:
                                _UI.SyncChat((ChatModel)com.Parameters[0]);
                                break;
                            case UICommand.AddMessageToChat:
                                _UI.AddMessageToChat((ChatModel)com.Parameters[0],
                                    (MessageModel)com.Parameters[1]);
                                break;
                            case UICommand.AddPersonToGroupChat:
                                _UI.AddPersonToGroupChat((GroupChatModel)com.Parameters[0],
                                    (PersonModel)com.Parameters[1]);
                                break;
                            case UICommand.UpdatePersonInGroupChat:
                                _UI.UpdatePersonInGroupChat((GroupChatModel)com.Parameters[0],
                                    (PersonModel)com.Parameters[1], (PersonModel)com.Parameters[2]);
                                break;
                            case UICommand.UpdateTopicInGroupChat:
                                _UI.UpdateTopicInGroupChat((GroupChatModel)com.Parameters[0],
                                    (string)com.Parameters[1]);
                                break;
                            case UICommand.RemovePersonFromGroupChat:
                                _UI.RemovePersonFromGroupChat((GroupChatModel)com.Parameters[0],
                                    (PersonModel)com.Parameters[1]);
                                break;
                            case UICommand.SetNetworkStatus:
                                _UI.SetNetworkStatus((string)com.Parameters[0]);
                                break;
                            case UICommand.SetStatus:
                                _UI.SetStatus((string)com.Parameters[0]);
                                break;
                            default:
#if LOG4NET
                                _Logger.Error("_Worker(): Unknown UICommand: "+com.Command);
#endif
                                break;
                        }
                    } catch (System.Runtime.Remoting.RemotingException e) {
#if LOG4NET
                        if (!_IsFrontendDisconnecting) {
                            // we didn't expect this problem
                            _Logger.Error("RemotingException in _Worker(), aborting FrontendManager thread...", e);
                            _Logger.Error("Inner-Exception: ", e.InnerException);
                        }
#endif
                        _Session.DeregisterFrontendUI(_UI);
                        return;
                    } catch (Exception e) {
#if LOG4NET
                        _Logger.Error("Exception in _Worker(), aborting FrontendManager thread...", e);
                        _Logger.Error("Inner-Exception: ", e.InnerException);
#endif
                        _Session.DeregisterFrontendUI(_UI);
                        return;
                    }
                } else {
                    // no better way?
                    Thread.Sleep(10);
                }
            }
        }
        
        private void _OnConfigChanged(object sender, EventArgs e)
        {
            Trace.Call(sender, e);
            // BUG: we should use some timeout here and only call the delegate
            // when the timeout is reached, else we flood the frontend for each
            // changed value in the config!
            try {
                if (_ConfigChangedDelegate != null) {
                    _ConfigChangedDelegate();
                }
            } catch (Exception ex) {
#if LOG4NET
                _Logger.Error(ex);
#endif
            }
        }
        
        private static string _(string msg)
        {
            return Mono.Unix.Catalog.GetString(msg);
        }
    }
}
