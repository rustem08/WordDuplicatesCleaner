# WordDuplicatesCleaner
The program removes duplicate words in input files (and between input files). The word can be made up of numbers and letters. The separator characters are not removed. The same words written in different registers are considered different words. All output files writes to .\output folder with same name as input files.

# Usage

DuplicatesCleaner.exe [-c maxThreadCount] [-t timeout_ms] ["inputPath"]

-c maximum number of parallel threads
filePath - full path to the file

# Test files

TestData.zip