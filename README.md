# SimpleVoteProtocolModel

It's the model of work of simple protocol of electronic voting.
This simple protocol consists of the following steps:

 - Each voter signs his bulletin with his key
 - Each voter encrypts his bulletin with the CEC key
 - Each voter sends his bulletin to the CEC
 - The CEC decrypts bulletins, verifies signatures, draws conclusions and publishes voting results
 
I use XOR algorithm for encryption and RSA for EDS.

# Database

This model uses DBMS SQLite. I also use Entity Framework as ORM framework in the code-first approach. The database model looks as follows:
![DB Image](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/Diagram.png)

# CEC UI
UI of the Central election commission has following list of WPF-windows:

 - Main window. In this window the CEC can view a list of registered persons (electors and voters):
 ![CEC Main window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/CEC%20main%20window.png)

- Persons window. In this window the CEC can register a new person (elector or voter):
![CEC Add person window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/CEC%20add%20person%20window.png)
- Votes window. In this window the CEC can view a list of votes:
![CEC Votes window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/CEC%20votes%20window.png)
- Results window. In this window the CEC can view the results of elections:
![CEC Results window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/CEC%20results%20window.png)
# Elector UI
Elector's UI has one Main window, where he can log in, choose the candidate and vote for him:
![Elector Main window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/Elector%20main%20window.png)
In the case of an unsuccessful login attempt the elector will see the following message:
![Elector not found window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/Elector%20not%20found%20window.png)
In the case of an successful login attempt the elector will see the following message:
![Elector found window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/Elector%20found%20window.png)
After successful login the elector will see the list of candidates:
![Elector signed window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/Elector%20signed%20window.png)
After voting the elector will see the following message:
![Elector voted window](https://github.com/ValeriiShmidt/SimpleVoteProtocolModelWPF/blob/master/Utils/Images/Elector%20voted%20window.png)
