#!/usr/bin/env python3

import sys
from collections import defaultdict


traktor_order = """

Loop > Loop In/Set Cue
Loop > Loop Out
Loop > Loop Size Selector
Loop > Loop Set
Loop > Loop Size Select + Set
Loop > Backward Loop Size Select + Set
Loop > Loop Active On 

 
"""


command_list1="""
        [CommandDescription(Categories.Mixer_XFader, "Auto X-Fade Left", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Mixer_XFader_AutoXFadeLeft = 2113,

        [CommandDescription(Categories.Mixer_XFader, "Auto X-Fade Right", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Mixer_XFader_AutoXFadeRight = 2114,


"""


file_in_unsorted = 'cmdr_commands_unsorted.txt'
file_in_sorted = 'traktor_commands_sorted.txt'
file_out_sorted = 'cmdr_commands_sorted.txt'
file_out_csharp = 'KnownCommands.cs''

with open(file_in_sorted) as f:
  print("Opening: %s" % (file_in_sorted))
  traktor_order = f.readlines()
  
with open(file_in_unsorted) as f:
  print("Opening: %s" % (file_in_unsorted))
  cmdr_list = f.readlines()

  
def print_line(line):
    print("|%s|" % (line))

    
def  remove_parentheses(key):
  if "(" in key:
    key = key.split("(")[0].strip()
  return key
  
    
  
#c2 = {}
c2 = defaultdict(list)
i=1
for line in cmdr_list:
    line = line.strip();
    #print_line(line)
    
    # ignore comments
    if line.startswith("//"):
        continue
        
    if line.startswith("#"):
        continue
        
    if line == "":
        continue
        
    if i == 1:
        line1 = line
        i = i+1
        continue
        
    else:
        i = 1
        line2 = line
        
        #print_line(line1)
        key = line1.split('"')[1].lower()
        key = remove_parentheses(key)  
          
        
        #print(line, key)
        ret = (line1, line2)
        
        # hack!
        #while key in c2:
        #  key = key + "."
        
        c2[key].append(ret) #= ret
        
        

def print_entry(value, file):
    print("%s\n%s\n\n" % (value[0], value[1]), file=file)

    
f_out = open(file_out_sorted, "w")    

c3 = c2.copy()
for line in traktor_order:
    line = line.strip();
    #print(line)
    if line == "":
        continue
        
    if ">" in line:
        line = line.split(">")[-1].strip()
        
    line = line.lower()    

    if line in c3.keys():
        for result in c3[line]:
          print_entry(result, f_out)
        del c3[line]
        #break
        
      
# print remaining commands unsorted
print("\n\n//\n// remaining commands (unsorted). To sort them, add names to traktor_commands_sorted.txt\n//\n\n", file=f_out)
for line in c3:
  for result in c3[line]:
    print_entry(result, f_out)
    
    
f_out.close()
    
print("All done. Copy-paste the contents of %s into %s, and remove last comma" % (file_out_sorted, file_out_csharp))
