from json import load

_TEMPLATE = u"""
    <Function Name="{Name}">
        <Arguments>{Arguments}</Arguments>
        <Return>{Return}</Return>
        <Description><![CDATA[{Description}]]></Description>
    </Function>
"""

funcs = load(open("e2funcs.utf8.json"), encoding="utf-8")
out = ['<?xml version="1.0" encoding="UTF-8"?>\n', "<FunctionList>\n"]
for func in funcs:
    out.append(_TEMPLATE.format(**func))
out.append("</FunctionList>")
open("functions.xml", "w+").write(''.join(out).encode('utf8'))
