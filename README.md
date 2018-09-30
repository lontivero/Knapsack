# Knapsack bitcoin coinjoin analysis

`Knapsack` coinjoin transactions are described in the paper [Anonymous CoinJoin Transactions with Arbitrary Values](https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf) and it is a novel way to mix transactions is such a way that the application of [well-known heuristics](https://cseweb.ucsd.edu/~smeiklejohn/files/imc13.pdf) is difficult in terms of complexity and also quite time consuming.

## Overview
The Knapsack model makes impractical the use of heuristics by introducing enough ambiguity into the mixed transaction. Let ilustrate this with an example:

Imagine 3 different people decide to send many to someone else.

```
# Alice Tx sending 4.59074 BTC
        [In]: 1.28381           [Out]: 4.59074
        [In]: 1.25411           [Out]: 0.10561
        [In]: 1.14851
        [In]: 0.89021
        [In]: 0.11971

# Bob Tx sending 1.02020 BTC
        [In]: 1.04301           [Out]: 1.02020
                                [Out]: 0.02281

# Carol Tx sending 0.04280 BTC
        [In]: 0.04301           [Out]: 0.04280
                                [Out]: 0.00021
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

An observer can see this transaction in the blockchain and analyze it searching for input-outputs combinations that make sense. In this case, if he searches for transactions analyzing at most 7 inputs and 4 outputs, it can find 8 candidate transactions.  
```
# 0
        [In]: 1.04301           [Out]: 1.02020
                                [Out]: 0.02281
        ##### 1.04301           ###### 1.04301
        ----------------------------------------------------------------------

 # 1
        [In]: 0.04301           [Out]: 0.02020
                                [Out]: 0.02281
        ##### 0.04301           ###### 0.04301
        ----------------------------------------------------------------------

        # 2
        [In]: 0.04301           [Out]: 0.04280
                                [Out]: 0.00021
        ##### 0.04301           ###### 0.04301
        ----------------------------------------------------------------------

 # 3
        [In]: 1.28381           [Out]: 3.65334
        [In]: 1.25411           [Out]: 1.02020
        [In]: 1.14851           [Out]: 0.02281
        [In]: 0.89021
        [In]: 0.11971
        ##### 4.69635           ###### 4.69635
        ----------------------------------------------------------------------

 # 4
        [In]: 1.04301           [Out]: 0.93740
                                [Out]: 0.08541
                                [Out]: 0.02020
        ##### 1.04301           ###### 1.04301
        ----------------------------------------------------------------------

# 5
        [In]: 1.28381           [Out]: 3.65334
        [In]: 1.25411           [Out]: 0.93740
        [In]: 1.14851           [Out]: 0.08541
        [In]: 0.89021           [Out]: 0.02020
        [In]: 0.11971
        ##### 4.69635           ###### 4.69635
        ----------------------------------------------------------------------

# 6
        [In]: 1.04301           [Out]: 1.02020
        [In]: 0.04301           [Out]: 0.02281
                                [Out]: 0.04280
                                [Out]: 0.00021
        ##### 1.08602           ###### 1.08602
        ----------------------------------------------------------------------

# 7
        [In]: 1.28381           [Out]: 3.65334
        [In]: 1.25411           [Out]: 1.02020
        [In]: 1.14851           [Out]: 0.02281
        [In]: 0.89021           [Out]: 0.04280
        [In]: 0.11971           [Out]: 0.00021
        [In]: 0.04301
        ##### 4.73936           ###### 4.73936
        ----------------------------------------------------------------------

# 8
        [In]: 1.04301           [Out]: 0.93740
        [In]: 0.04301           [Out]: 0.08541
                                [Out]: 0.02020
                                [Out]: 0.04280
                                [Out]: 0.00021
        ##### 1.08602           ###### 1.08602
        ----------------------------------------------------------------------
```