namespace build;

public static class MacAppBundle
{
    public const string Entitlements = """
                                       <?xml version="1.0" encoding="UTF-8"?>
                                       <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                                       <plist version="1.0">
                                       <dict>
                                           <key>com.apple.security.cs.allow-jit</key>
                                           <true/>
                                           <key>com.apple.security.cs.allow-unsigned-executable-memory</key>
                                           <true/>
                                           <key>com.apple.security.cs.disable-library-validation</key>
                                           <true/>
                                           <key>com.apple.security.cs.allow-dyld-environment-variables</key>
                                           <true/>
                                       </dict>
                                       </plist>
                                       """;

    private const string InfoPList = """
                                     <?xml version="1.0" encoding="UTF-8"?>
                                     <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                                     <plist version="1.0">
                                       <dict>
                                         <key>CFBundleDevelopmentRegion</key>
                                     	<string>en</string>
                                         <key>CFBundleIconFile</key>
                                         <string>{0}.icns</string>
                                         <key>CFBundleIdentifier</key>
                                         <string>com.{0}</string>
                                     	<key>CFBundleDisplayName</key>
                                         <string>{0}</string>
                                         <key>CFBundleName</key>
                                         <string>{0}</string>
                                         <key>CFBundleVersion</key>
                                         <string>{1}</string>
                                         <key>LSMinimumSystemVersion</key>
                                         <string>13.00</string>
                                         <key>CFBundleExecutable</key>
                                         <string>{0}</string>
                                         <key>CFBundleInfoDictionaryVersion</key>
                                         <string>6.0</string>
                                         <key>CFBundlePackageType</key>
                                         <string>APPL</string>
                                         <key>CFBundleShortVersionString</key>
                                         <string>{1}</string>
                                     	<key>CFBundleSupportedPlatforms</key>
                                     	<array>
                                     		<string>MacOSX</string>
                                     	</array>
                                     	<key>LSApplicationCategoryType</key>
                                     	<string>public.app-category.utilities</string>
                                     	<key>NSHighResolutionCapable</key>
                                     	<true />
                                     	<key>NSHumanReadableCopyright</key>
                                     	<string>{2}</string>
                                       </dict>
                                     </plist>
                                     """;

    private const string MultiArchEntryScript = """
                                       #!/bin/bash

                                       DIR=$(dirname "$0")
                                       ARM64=$(sysctl -ni hw.optional.arm64)

                                       if [[ "$ARM64" == 1 ]]; then
                                           export PATH="${{DIR}}/osx-arm64/":"${{PATH}}"
                                       else
                                           export PATH="${{DIR}}/osx-x64/":"${{PATH}}"
                                       fi

                                       exec "{0}" $@
                                       """;

    public static string GetInfoPList(string productName, string version, string copyright)
    {
        return string.Format(InfoPList, productName, version, copyright);
    }

    public static string GetMultiArchEntryScript(string productName)
    {
        return string.Format(MultiArchEntryScript, productName);
    }
}