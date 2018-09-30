# Knapsack bitcoin coinjoin analysis

**WARINING:** this is draft for learning and keeping record of that.


`Knapsack` coinjoin transactions are described in the paper [Anonymous CoinJoin Transactions with Arbitrary Values](https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf) and it is a novel way to mix transactions is such a way that the application of [well-known heuristics](https://cseweb.ucsd.edu/~smeiklejohn/files/imc13.pdf) is difficult in terms of complexity and also quite time consuming.

## Overview
The Knapsack model makes impractical the use of heuristics by introducing enough ambiguity into the mixed transaction. Let ilustrate this with an example:

Imagine 3 different people decide to send many to someone else.

```
# Alice Tx sending 4.59074 BTC
        [In]: 1.28381           [Out]: 4.59074
        [In]: 1.25411           [Out]: 0.10561   <--- change
        [In]: 1.14851
        [In]: 0.89021
        [In]: 0.11971

# Bob Tx sending 1.02020 BTC
        [In]: 1.04301           [Out]: 1.02020
                                [Out]: 0.02281   <--- change

# Carol Tx sending 0.04280 BTC
        [In]: 0.04301           [Out]: 0.04280
                                [Out]: 0.00021   <--- change
```

They can mix theirs transactions as follow:

```
# Alice, Bob and Carol mixed TX
        [In]: 1.28381           [Out]: 3.65334
        [In]: 1.25411           [Out]: 1.02020
        [In]: 1.14851           [Out]: 0.93740
        [In]: 0.89021           [Out]: 0.08541
        [In]: 0.11971           [Out]: 0.02020
        [In]: 1.04301           [Out]: 0.02281
        [In]: 0.04301           [Out]: 0.04280
                                [Out]: 0.00021
```

An observer can see this transaction in the blockchain and analyze it searching for input-outputs combinations that make sense. In this case, if he searches for transactions analyzing at most 7 inputs and 5 outputs, it can find 9 candidate transactions.  

<details>
  <summary>(Expand) <b>CoinJoin Transaction analysis details</b></summary>

```
# 0
        [In]: 1.04301           [Out]: 1.02020
                                [Out]: 0.02281
        ##### 1.04301           ###### 1.04301
        ----------------------------------------------

 # 1
        [In]: 0.04301           [Out]: 0.02020
                                [Out]: 0.02281
        ##### 0.04301           ###### 0.04301
        ----------------------------------------------

        # 2
        [In]: 0.04301           [Out]: 0.04280
                                [Out]: 0.00021
        ##### 0.04301           ###### 0.04301
        ----------------------------------------------

 # 3
        [In]: 1.28381           [Out]: 3.65334
        [In]: 1.25411           [Out]: 1.02020
        [In]: 1.14851           [Out]: 0.02281
        [In]: 0.89021
        [In]: 0.11971
        ##### 4.69635           ###### 4.69635
        ----------------------------------------------

 # 4
        [In]: 1.04301           [Out]: 0.93740
                                [Out]: 0.08541
                                [Out]: 0.02020
        ##### 1.04301           ###### 1.04301
        ----------------------------------------------

# 5
        [In]: 1.28381           [Out]: 3.65334
        [In]: 1.25411           [Out]: 0.93740
        [In]: 1.14851           [Out]: 0.08541
        [In]: 0.89021           [Out]: 0.02020
        [In]: 0.11971
        ##### 4.69635           ###### 4.69635
        ----------------------------------------------

# 6
        [In]: 1.04301           [Out]: 1.02020
        [In]: 0.04301           [Out]: 0.02281
                                [Out]: 0.04280
                                [Out]: 0.00021
        ##### 1.08602           ###### 1.08602
        ----------------------------------------------

# 7
        [In]: 1.28381           [Out]: 3.65334
        [In]: 1.25411           [Out]: 1.02020
        [In]: 1.14851           [Out]: 0.02281
        [In]: 0.89021           [Out]: 0.04280
        [In]: 0.11971           [Out]: 0.00021
        [In]: 0.04301
        ##### 4.73936           ###### 4.73936
        ----------------------------------------------

# 8
        [In]: 1.04301           [Out]: 0.93740
        [In]: 0.04301           [Out]: 0.08541
                                [Out]: 0.02020
                                [Out]: 0.04280
                                [Out]: 0.00021
        ##### 1.08602           ###### 1.08602
        ----------------------------------------------
```
</details>


These 9 subtransactions are not all the possible transactions that can be extracted from the coinjoined tx but just a subset of those with `count(inputs) <= 7 && count(outputs) <= 5`. 

Note that in this example the 3 original transactions were founded (not in their original form), however, it is not easy for an observer to extract accurated info. 

For example, imagine we know the Bob's address containing the **1.04301 BTC** that he uses in his transaction. After analyzing the coinjoined tx we see that that same coin participates in 4 possible subtransactions (#0, #4, #6 y #8). Let see what happens with the heristics:

The first heuristic (H1) 
> All the inputs in a transaction belong to the same wallet.

In this case H1 fails in 3 out 4 cases. See #8 for example, it could be interpreted as Bob sending **1.08602 BTC** to someone else and **0.00021** back as change. However, the other input (0.04301 BTC) is not a Bob input but a carol one.
4.69635

The second heuristic (H2)
> The change address is belong to the same user (wallet) and it identifiable because is the one with the smallest amount.

See the output with value **0.02281 BTC**, it is the one with the smallest amount in 3 subtransactions (#0, #1 and #3) that contains coins from all participants.

## How to run

<aside class="notice">Running with values greater than 6 can take several hours!!</aside>

```
$ git clone https://github.com/lontivero/Knapsack.git
$ cd src
$ dotnet run <number-of-participants>
```

## Examples

In the `data` folder you can find files with simulated knapsack coinjoin transactions and the corresponding post analysis.

* [Knapsack CoinJoin with 3 participants](https://github.com/lontivero/Knapsack/blob/master/data/knapsack-3-participants.txt)
* [Knapsack CoinJoin with 4 participants](https://github.com/lontivero/Knapsack/blob/master/data/knapsack-4-participants.txt)
* [Knapsack CoinJoin with 5 participants](https://github.com/lontivero/Knapsack/blob/master/data/knapsack-5-participants.txt)
* [Knapsack CoinJoin with 6 participants](https://github.com/lontivero/Knapsack/blob/master/data/knapsack-6-participants.txt)
* Knapsack CoinJoin with 7 participants
* Knapsack CoinJoin with 8 participants
* Knapsack CoinJoin with 9 participants

## Times
Finding the all the possible combinations (subtransactions) hidden in the mixed transaction is a extremly high time consuming task which difficulty increases exponentially with the number of participants in a coinjoin.

Even for a subset of subtransactions, as is the case in this examples, the time required is a lot:

* 3 participants:  **00:00:02**
* 4 participants:  **00:00:02**
* 5 participants:  **00:00:51**
* 6 participants:  **00:05:39**
* 7 participants:  **more than 3 hours and still running!!!**


This is because the number of all the possible combinations is 

Just as an example, in the simulation with 7 participants, the total number of combinations is:
```python
>>> (n, m) = (28, 20)
>>> '%d' % (((n/math.log(n,2))**n)*((m/math.log(m,2))**m)*n*m)

69013027878983317231997877419510118251132616704
```

This can be optimized using the algorithm described in the original paper to reduce it to:
```
>>> (2**28) * 20

5368709120

```

This was not tested/researched yet.
