# Tuple Rewriter

*Created for JetBrains intership projects 2025-2026*

Given a small AST, the program refactors every tuple literal into a named type. It also has a built in parser that turns text into an AST, and a printer that turns the AST back into text.

## Installation

1. Clone the repository
   
`git clone https://github.com/markovolimango/TupleRewriter.git`

2. Navigate to the folder
   
`cd TupleRewriter`

## Usage

### Running the tests

`dotnet test`

### Refactoring a file

1. Run the project

`dotnet run --project TupleRewriter`

2. Follow the instructions in the terminal (enter the path to your file and the new type name) and your file is reformated!
