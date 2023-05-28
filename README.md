# OliPwn
[![forthebadge](https://forthebadge.com/images/badges/gluten-free.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/no-ragrets.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/powered-by-black-magic.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/uses-badges.svg)](https://forthebadge.com)

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
Here is an example of a template file for ois_swaps:
```
#include <iostream>

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
Here is an example of using OliPwn to get the variable N from ois_swaps:
```
Username:flame
Password:iLovePuppies<3
Task name:ois_swaps
Variable name:N
Variable max value:1000000
Invalid output example:0
Template file name:swaps.cpp
Checking ids: [1026483, 1026484, 1026485, 1026486, 1026487, 1026488, 1026489, 1026490, 1026492, 1026493]
N/1 mod 4: 0 1 3 0 1 2 3 0 1 1 2 3 1 3 0 0 0 3 0 1 0 0 0 0 0 0 0 0 3 2
N/4 mod 4: 1 1 0 1 1 1 1 2 0 0 0 0 3 3 1 2 2 1 1 1 2 0 0 0 0 0 0 0 3 3
N/16 mod 4: 0 0 0 0 0 0 0 0 0 2 2 2 1 1 2 0 0 0 1 3 0 2 0 2 0 0 0 0 3 3
N/64 mod 4: 0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 2 2 2 2 2 2 2 1 0 1 1 1 1 0 0
N/256 mod 4: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 3 3 3 1 2 3 2 1 1 2 2 2 2 2 2
N/1024 mod 4: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 3 0 0 0 0 0 0 0
N/4096 mod 4: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 0 0 2 0 0 0 0 0 0
N/16384 mod 4: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2 0 2 1 1 1 1 1 1
N/65536 mod 4: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 3 3 3 3 3 3 3 3
N/262144 mod 4: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 3 3 3 3 3 3
4 5 3 4 5 6 7 8 1 97 98 99 93 95 100 5000 5000 4999 4500 4789 5000 100000 200000 500000 1000000 1000000 1000000 1000000 999999 999998
```
