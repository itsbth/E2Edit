from json import loads, dumps
from re import findall

# open the gmod dump
luadump = loads(open("luadump.txt", "r").read())

# iterate through all functions in the lua dump, giving them empty descriptions
for k, v in luadump.items():
    luadump[k]["desc"] = ""

# parse the wiki markup
wiki = {}
wikidump = open("wikidump.txt", "r").read()

# find all functions
for match in findall(r"\|.*?(?:(\w+)\:)?(\w+)\((.+)\).*?\|\|.+\|\| (.+)", wikidump):
    # generate signature for the function
    signature = match[1] + "(" + (match[0].replace(",", "").lower() + ":" if len(match[0]) > 0 else "") + match[2].replace(",", "").lower() + ")"
    # match it up to the lua dump and assign the description
    if signature in luadump:
        luadump[signature]["desc"] = match[3]

file = open("e2funcs.utf8.json", "w")
file.write(dumps(luadump).encode('utf-8'))
