# OliPwn
A simple tool for exfiltrating variables from training.olinfo.it
## How it works
When making a submission, there are a few pieces of data that we can read and manipulate from inside the program.
These include:
* Answer correctness
* Error status
* Time used
* Memory used
OliPwn sends numerous queries in which it checks a piece of the variable to exfiltrate's value and according to that alters these fields.
Since there usually aren't enough combinations in one query for every value of the variable, this process is generally repeated until we have our complete variable.

The current speed is 2 bits/query, however that can be somewhat improved by altering time and memory in a better way.
## How to use
Once you open the program you'll be asked to give
* An username
* A password
* The name of the task on which to work on (you generally find that in https://training.olinfo.it/#/task/TASK_NAME/...)
* The name of the variable to exfiltrate as it appears in code (you COULD(?) also use expressions, like `(V[69%N] + A)`, this will just be pasted in your code)
* The maximum value that variable can reach (just used for performance, when in doubt 9223372036854775807)
* An invalid output (This is not an output as much as a return value, generally something like -2, just be sure it's not an accidental solution of a testcase)
* The path of a template file
### What's a template file?
It's just a file with all the required code of the task + a marker for when to do the variable check.
This is an example of a template file for ois_swaps:
```
#include <fstream>
#include <iostream>
#include <vector>

using namespace std;

int main() {
    int N;
    cin >> N;
    
    {$CHECK$}
    
    return 0;
}
```
You can very easily make one from the task's template files.
The program will then proceed to make the necessary queries and will return a list of variables, one for each testcase.
