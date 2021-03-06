SUBDIRS =	SmartIrc4net

OUTPUT_DIR = $(top_builddir)/bin/$(PROFILE)
OBJECT_DIR = $(OUTPUT_DIR)/obj
SOURCE_PATTERNS = *.cs */*.cs */*/*.cs */*/*/*.cs */*/*/*/*.cs */*/*/*/*/*.cs
XBUILD_FLAGS += /p:Configuration=Debug /p:SignAssembly=false /p:WarningLevel=0 /p:OutputPath=$(abspath $(OUTPUT_DIR)) /p:BaseIntermediateOutputPath=$(abspath $(OBJECT_DIR))/ /p:IntermediateOutputPath=$(abspath $(OBJECT_DIR))/ /p:DocumentationFile=

JSON_SUBDIR = Newtonsoft.Json
JSON_SRCDIR = $(srcdir)/$(JSON_SUBDIR)/Src/Newtonsoft.Json
JSON_SOURCE_FILES = $(foreach pattern, $(SOURCE_PATTERNS), $(wildcard $(JSON_SRCDIR)/$(pattern)))
JSON_CLEAN_FILES = $(OBJECT_DIR)/Newtonsoft.Json.Dynamic.snk
JSON_ASSEMBLY_NAME = Newtonsoft.Json.dll
JSON_ASSEMBLY_TARGET = $(OUTPUT_DIR)/$(JSON_ASSEMBLY_NAME)
JSON_BUILD_FILE = $(JSON_SRCDIR)/Newtonsoft.Json.csproj
JSON_XBUILD_FLAGS = $(XBUILD_FLAGS)

TWITTERIZER_SUBDIR = Twitterizer
TWITTERIZER_SRCDIR = $(srcdir)/$(TWITTERIZER_SUBDIR)/Twitterizer2
TWITTERIZER_SOURCE_FILES = $(foreach pattern, ../*.cs $(SOURCE_PATTERNS), $(wildcard $(TWITTERIZER_SRCDIR)/$(pattern)))
TWITTERIZER_EXTRA_FILES = $(TWITTERIZER_SRCDIR)/Twitterizer2.snk $(TWITTERIZER_SRCDIR)/../GettingStarted.txt $(TWITTERIZER_SRCDIR)/../Json.NET.license.txt $(TWITTERIZER_SRCDIR)/../Twitterizer2.license.txt
TWITTERIZER_CLEAN_FILES = \
	$(OUTPUT_DIR)/.license.txt \
	$(OUTPUT_DIR)/Twitterizer2.license.txt \
	$(OUTPUT_DIR)/Json.NET.license.txt \
	$(OUTPUT_DIR)/GettingStarted.txt
TWITTERIZER_ASSEMBLY_NAME = Twitterizer2.dll
TWITTERIZER_ASSEMBLY_TARGET = $(OUTPUT_DIR)/$(TWITTERIZER_ASSEMBLY_NAME)
TWITTERIZER_BUILD_FILE = $(TWITTERIZER_SRCDIR)/Twitterizer2.csproj
TWITTERIZER_XBUILD_FLAGS = $(XBUILD_FLAGS) /p:PostBuildEvent=

JABBER_NET_SUBDIR = jabber-net
JABBER_NET_SRCDIR = $(srcdir)/$(JABBER_NET_SUBDIR)
JABBER_NET_SOURCE_FILES = $(foreach pattern, $(SOURCE_PATTERNS), $(wildcard $(JABBER_NET_SRCDIR)/$(pattern)))
JABBER_NET_EXTRA_FILES = $(wildcard $(JABBER_NET_SRCDIR)/jabber/*/*.bmp) $(wildcard $(JABBER_NET_SRCDIR)/jabber/*/*.resx)
JABBER_NET_CLEAN_FILES = $(wildcard $(OBJECT_DIR)/*.bmp)
JABBER_NET_ASSEMBLY_NAME = jabber-net.dll
JABBER_NET_ASSEMBLY_TARGET = $(OUTPUT_DIR)/$(JABBER_NET_ASSEMBLY_NAME)
JABBER_NET_BUILD_FILE = $(JABBER_NET_SRCDIR)/2005-jabber-net.csproj
JABBER_NET_XBUILD_FLAGS = $(XBUILD_FLAGS)

DB4O_SUBDIR = db4o-net
DB4O_SRCDIR = $(srcdir)/$(DB4O_SUBDIR)/Db4objects.Db4o
DB4O_SOURCE_FILES = $(foreach pattern, $(SOURCE_PATTERNS), $(wildcard $(DB4O_SRCDIR)/$(pattern)))
DB4O_ASSEMBLY_NAME = Db4objects.Db4o.dll
DB4O_ASSEMBLY_TARGET = $(OUTPUT_DIR)/$(DB4O_ASSEMBLY_NAME)
DB4O_BUILD_FILE = $(DB4O_SRCDIR)/Db4objects.Db4o-2008.csproj
DB4O_XBUILD_FLAGS = $(XBUILD_FLAGS) /property:DefineConstants="NET_3_5,MONO,EMBEDDED"

DB4O_INSTR_SRCDIR = $(srcdir)/$(DB4O_SUBDIR)/Db4objects.Db4o.Instrumentation
DB4O_INSTR_SOURCE_FILES = $(foreach pattern, $(SOURCE_PATTERNS), $(wildcard $(DB4O_INSTR_SRCDIR)/$(pattern)))
DB4O_INSTR_ASSEMBLY_NAME = Db4objects.Db4o.Instrumentation.dll
DB4O_INSTR_ASSEMBLY_TARGET = $(OUTPUT_DIR)/$(DB4O_INSTR_ASSEMBLY_NAME)
DB4O_INSTR_BUILD_FILE = $(DB4O_INSTR_SRCDIR)/Db4objects.Db4o.Instrumentation-2008.csproj
DB4O_INSTR_XBUILD_FLAGS = $(DB4O_XBUILD_FLAGS)

DB4O_NQ_SRCDIR = $(srcdir)/$(DB4O_SUBDIR)/Db4objects.Db4o.NativeQueries
DB4O_NQ_SOURCE_FILES = $(foreach pattern, $(SOURCE_PATTERNS), $(wildcard $(DB4O_NQ_SRCDIR)/$(pattern)))
DB4O_NQ_ASSEMBLY_NAME = Db4objects.Db4o.NativeQueries.dll
DB4O_NQ_ASSEMBLY_TARGET = $(OUTPUT_DIR)/$(DB4O_NQ_ASSEMBLY_NAME)
DB4O_NQ_BUILD_FILE = $(DB4O_NQ_SRCDIR)/Db4objects.Db4o.NativeQueries-2008.csproj
DB4O_NQ_XBUILD_FLAGS = $(DB4O_XBUILD_FLAGS)

# magic automake variables
if ENABLE_ENGINE_TWITTER
EXTRA_TWITTER_LIBS = \
	$(JSON_ASSEMBLY_TARGET) $(JSON_ASSEMBLY_TARGET).mdb \
	$(TWITTERIZER_ASSEMBLY_TARGET) $(TWITTERIZER_ASSEMBLY_TARGET).mdb
endif
if ENABLE_ENGINE_XMPP
EXTRA_XMPP_LIBS = \
	$(JABBER_NET_ASSEMBLY_TARGET) $(JABBER_NET_ASSEMBLY_TARGET).mdb
endif
if BUNDLE_DB4O
EXTRA_DB4O_LIBS = \
	$(DB4O_ASSEMBLY_TARGET) $(DB4O_ASSEMBLY_TARGET).mdb
#	$(DB4O_INSTR_ASSEMBLY_TARGET) $(DB4O_INSTR_ASSEMBLY_TARGET).mdb \
#	$(DB4O_NQ_ASSEMBLY_TARGET) $(DB4O_NQ_ASSEMBLY_TARGET).mdb
endif
pkgappdir = $(pkglibdir)
pkgapp_DATA = $(EXTRA_TWITTER_LIBS) $(EXTRA_XMPP_LIBS) $(EXTRA_DB4O_LIBS)

EXTRA_DIST = \
	 $(JSON_SOURCE_FILES) $(JSON_BUILD_FILE) $(JSON_SRCDIR)/Dynamic.snk \
	 $(TWITTERIZER_SOURCE_FILES) $(TWITTERIZER_EXTRA_FILES) $(TWITTERIZER_BUILD_FILE) \
	 $(JABBER_NET_SOURCE_FILES) $(JABBER_NET_EXTRA_FILES) $(JABBER_NET_BUILD_FILE) \
	 $(DB4O_SOURCE_FILES) $(DB4O_BUILD_FILE) \
	 $(DB4O_INSTR_SOURCE_FILES) $(DB4O_INSTR_BUILD_FILE) \
	 $(DB4O_NQ_SOURCE_FILES) $(DB4O_NQ_BUILD_FILE)

CLEANFILES = \
	$(JSON_CLEAN_FILES) \
	$(JABBER_NET_CLEAN_FILES) \
	$(TWITTERIZER_CLEAN_FILES)
# end of magic

$(JSON_ASSEMBLY_TARGET) $(JSON_ASSEMBLY_TARGET).mdb: $(JSON_BUILD_FILE) $(JSON_SOURCE_FILES)
	$(XBUILD) $(JSON_XBUILD_FLAGS) $(JSON_BUILD_FILE)

$(TWITTERIZER_ASSEMBLY_TARGET) $(TWITTERIZER_ASSEMBLY_TARGET).mdb: $(TWITTERIZER_BUILD_FILE) $(TWITTERIZER_SOURCE_FILES)
	$(XBUILD) $(TWITTERIZER_XBUILD_FLAGS) $(TWITTERIZER_BUILD_FILE)

$(JABBER_NET_ASSEMBLY_TARGET) $(JABBER_NET_ASSEMBLY_TARGET).mdb: $(JABBER_NET_BUILD_FILE) $(JABBER_NET_SOURCE_FILES)
	$(XBUILD) $(JABBER_NET_XBUILD_FLAGS) $(JABBER_NET_BUILD_FILE)

$(DB4O_ASSEMBLY_TARGET) $(DB4O_ASSEMBLY_TARGET).mdb: $(DB4O_BUILD_FILE) $(DB4O_SOURCE_FILES)
	$(XBUILD) $(DB4O_XBUILD_FLAGS) $(DB4O_BUILD_FILE)

$(DB4O_INSTR_ASSEMBLY_TARGET) $(DB4O_INSTR_ASSEMBLY_TARGET).mdb: $(DB4O_INSTR_BUILD_FILE) $(DB4O_INSTR_SOURCE_FILES)
	$(XBUILD) $(DB4O_INSTR_XBUILD_FLAGS) $(DB4O_INSTR_BUILD_FILE)

$(DB4O_NQ_ASSEMBLY_TARGET) $(DB4O_NQ_ASSEMBLY_TARGET).mdb: $(DB4O_NQ_BUILD_FILE) $(DB4O_NQ_SOURCE_FILES)
	$(XBUILD) $(DB4O_NQ_XBUILD_FLAGS) $(DB4O_NQ_BUILD_FILE)

clean-local:
if ENABLE_ENGINE_TWITTER
	$(XBUILD) $(JSON_XBUILD_FLAGS) /t:Clean $(JSON_BUILD_FILE)
	$(XBUILD) $(TWITTERIZER_XBUILD_FLAGS) /t:Clean $(TWITTERIZER_BUILD_FILE)
endif
if ENABLE_ENGINE_XMPP
	$(XBUILD) $(JABBER_NET_XBUILD_FLAGS) /t:Clean $(JABBER_NET_BUILD_FILE)
endif
if BUNDLE_DB4O
	$(XBUILD) $(DB4O_XBUILD_FLAGS) /t:Clean $(DB4O_BUILD_FILE)
#	$(XBUILD) $(DB4O_INSTR_XBUILD_FLAGS) /t:Clean $(DB4O_INSTR_BUILD_FILE)
#	$(XBUILD) $(DB4O_NQ_XBUILD_FLAGS) /t:Clean $(DB4O_NQ_BUILD_FILE)
endif
