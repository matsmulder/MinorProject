import os
from os.path import isfile, isdir

fileLocations = set()

def checkFile(f):
    count = 0
    found = False
    try:
        with open(f,"r") as fp:
            for line in fp:
                count+=1
                if query in line:
                    found = True
                    print "Found at %s:%d" % (f, count)
            
        if found:
            fileLocations.add("/".join(f.split("/")[:-1]))
    except IOError:
        print "Could not read file %s" % f

def checkDir(d):
    for f in os.listdir(d):
        if isfile(d+"/"+f):
            checkFile(d+"/"+f)
        elif isdir(d+"/"+f):
            checkDir(d+"/"+f)
        else:
            print "Could not read %s" % f
            
if __name__=="__main__":
    query = raw_input("Query: ")
    checkDir(".")
    if len(fileLocations)!=0:
		op = raw_input("Open file locations? (y/n) ")
		if op=="y":
			import subprocess
			for f in fileLocations:
				subprocess.Popen('explorer %s' % os.getcwd()+f[1:].replace("/", "\\"))
				
    os.system("pause")