out = {"{"} -- hueg json table
for k, v in pairs(wire_expression2_funcs) do
    if string.sub(v[1], 1, 3) ~= "op:" then -- don't include operators
        id, args = string.match(v[1], "(%w+)%(([^%)]*)%)") -- pull identifier and arguments out of signature
        table.insert(out, ([[["Name": "%q","Arguments":"%q","Return":"%q"]]]):format(id, args, v[2])) -- add to the json table dohicky
    end
end
table.insert(out, "}")
file.Write("funcs.txt", table.concat(out))
