using Irony.Parsing;

namespace E2ParserLib
{
    public class E2Grammar : Grammar
    {
        public E2Grammar()
        {
            LineTerminators = "\r\n";
            var program = new NonTerminal("Program");
            var directiveList = new NonTerminal("DirectiveList");
            var directive = new NonTerminal("Directive");
            var directiveName = new IdentifierTerminal("DirectiveName");
            var directiveBody = new NonTerminal("DirectiveBody");
            var statementList = new NonTerminal("StatementList");
            var statement = new NonTerminal("Statement");
            var assignment = new NonTerminal("Assignment");
            var expression = new NonTerminal("Expression");
            var parenExpression = new NonTerminal("ParenExpression");
            var methodCall = new NonTerminal("MethodCall");
            var functionCall = new NonTerminal("FunctionCall");
            var argumentList = new NonTerminal("ArgumentList");
            var argumentTail = new NonTerminal("ArgumentTail");
            var @operator = new NonTerminal("Operator");
            var operation = new NonTerminal("Operation");
            var identifier = new IdentifierTerminal("Identifier");
            var @string = new StringLiteral("String", "\"");
            var number = new NumberLiteral("Number");
            var ifStatement = new NonTerminal("IfStatement");
            var elseIfStatement = new NonTerminal("ElseIfStatement");
            var elseStatement = new NonTerminal("ElseIfStatement");
            var whileStatement = new NonTerminal("WhileStatement");
            var comment = new CommentTerminal("comment", "#", new[] { "\n" });
            var dcom = new CommentTerminal("commentCheat", "@", new[] { "\n" });
            var arrayAccess = new NonTerminal("ArrayAccess");
            var changed = new NonTerminal("changed");

            Root = program;

            NonGrammarTerminals.Add(comment);
            NonGrammarTerminals.Add(dcom);
            MarkReservedWords("if", "while", "else", "elseif", "for");

            RegisterBracePair("{", "}");
            RegisterBracePair("(", ")");

            MarkTransient(expression, statement);

            program.Rule = /* directiveList + */statementList;

            //directiveList.Rule = MakePlusRule(directiveList, null, directive);
            //directiveBody.Rule = new CommentTerminal()
            //directive.Rule = ToTerm("@") + directiveName + directiveBody;

            statementList.Rule = MakePlusRule(statementList, null, statement);
            statement.Rule = methodCall | functionCall | assignment | ifStatement | whileStatement;

            expression.Rule = operation | @string | number | methodCall | functionCall | identifier | parenExpression | arrayAccess | changed;

            parenExpression.Rule = ToTerm("(") + expression + ")";

            methodCall.Rule = expression + ":" + identifier + "(" + argumentList + ")";

            functionCall.Rule = identifier + "(" + argumentList + ")";

            argumentList.Rule = MakeStarRule(argumentList, ToTerm(","), expression);

            operation.Rule = expression + @operator + expression;

            @operator.Rule = ToTerm("+") | "-" | "*" | "/" | "%" | "==" | "!=" | "<" | ">" | "=>" | "=<" | "&" | "|";

            changed.Rule = ToTerm("~") + identifier;

            assignment.Rule = identifier + "=" + expression;

            ifStatement.Rule = ToTerm("if") + parenExpression + "{" + statementList + "}" + (Empty | elseIfStatement | elseStatement);

            elseIfStatement.Rule = ToTerm("elseif") + parenExpression + "{" + statementList + "}" + (Empty | elseIfStatement | elseStatement);
            elseStatement.Rule = ToTerm("else") + "{" + statementList + "}";

            whileStatement.Rule = ToTerm("while") + parenExpression + "{" + statementList + "}";

            arrayAccess.Rule = identifier + "[" + expression + (Empty | (ToTerm(",") + identifier)) + "]";
            
        }
    }
}
