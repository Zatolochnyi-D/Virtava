script_path=$(cd $(dirname $0) && pwd)
package_name="VirtavaArkitBlendshapesForCSharpClient.0.0.1.nupkg"
cd $script_path
dotnet pack
dotnet nuget push "VirtavaArkitBlendshapesForCSharpClient/bin/Release/$package_name" --source Virtava
dotnet nuget locals global-packages --clear