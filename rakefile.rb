require 'etc'
require 'fileutils'

BAZAAR_WHOAMI = Etc.getlogin
BAZAAR_ROOT_PATH = "/home/briankernighan/IdeaProjects"
BAZAAR_FOLDER_NAME = "Bazaar";
BAZAAR_TESTING_FODLER_NAME = "Bazaar.Testing"

desc "Clean Bazaar Project"
task :clean_bazaar do
	sh "sudo dotnet clean"
end

desc "Publish Bazaar Project"
task :publish_bazaar do
	testingFolder = BAZAAR_ROOT_PATH + "/" + BAZAAR_TESTING_FODLER_NAME
	unless File.directory?(testingFolder)
	  FileUtils.mkdir_p(testingFolder)
	end
	sh "sudo dotnet publish --framework netcoreapp2.1 --runtime Portable --output " + BAZAAR_ROOT_PATH + "/" + BAZAAR_TESTING_FODLER_NAME + "/" + "#{Time.new.strftime("%d.%m.%Y-%H:%M:%S")}"
	sh "sudo chown -R " + BAZAAR_WHOAMI + ":" + BAZAAR_WHOAMI + " " + BAZAAR_ROOT_PATH + "/" + BAZAAR_FOLDER_NAME
	sh "sudo chown -R " + BAZAAR_WHOAMI + ":" + BAZAAR_WHOAMI + " " + BAZAAR_ROOT_PATH + "/" + BAZAAR_TESTING_FODLER_NAME
end

task :default => [:clean_bazaar, :publish_bazaar]

