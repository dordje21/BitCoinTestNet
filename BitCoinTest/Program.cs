
using System.Transactions;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

Key privateKey = Key.Parse("cScHSaYbJfZchsqc6GQqpYv8eiu6t5DGFvz9g92wqaCwVbNJ2W67", Network.TestNet); 
PubKey publicKey = privateKey.PubKey;
Console.WriteLine(publicKey);
Console.WriteLine(publicKey.GetAddress(ScriptPubKeyType.Legacy, Network.TestNet));
BitcoinSecret testNetPrivateKey = privateKey.GetBitcoinSecret(Network.TestNet);
Console.WriteLine(testNetPrivateKey);

var network = Network.TestNet;

var client = new QBitNinjaClient(network);
var transactionId = uint256.Parse("b2c9da5658926204043bcbdd98d623b982e5267b6876d9a892d21ddfbf20c2e4");
var transactionResponse = client.GetTransaction(transactionId).Result;

Console.WriteLine(transactionResponse.TransactionId); // 0acb6e97b228b838049ffbd528571c5e3edd003f0ca8ef61940166dc3081b78a
Console.WriteLine(transactionResponse.Block.Confirmations); // 91

var receivedCoins = transactionResponse.ReceivedCoins;
OutPoint outPointToSpend = null;
foreach (var coin in receivedCoins)
{
    if (coin.TxOut.ScriptPubKey == privateKey.GetAddress(ScriptPubKeyType.Legacy, network).ScriptPubKey)
    {
        outPointToSpend = coin.Outpoint;
    }
}
if (outPointToSpend == null)
{
    throw new Exception("TxOut doesn't contain our ScriptPubKey");
}
Console.WriteLine("We want to spend {0}. outpoint:", outPointToSpend.N + 1);

 
 
    var transaction = NBitcoin.Transaction.Create(network);
    transaction.Inputs.Add(new TxIn()
    {
        PrevOut = outPointToSpend
    });
 

var hallOfTheMakersAddress = BitcoinAddress.Create("muWRKSHdxw9is6R8ZcsNNCMcWbwcbCBcUi", Network.TestNet);

transaction.Outputs.Add(Money.Coins(0.001m), hallOfTheMakersAddress.ScriptPubKey);

var minerFee = new Money(0.00007m, MoneyUnit.BTC);

// How much you want to get back as change
var hallOfTheMakersAmount = new Money(0.001m, MoneyUnit.BTC);
var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
var changeAmount = txInAmount - hallOfTheMakersAmount - minerFee;


transaction.Outputs.Add(hallOfTheMakersAmount, hallOfTheMakersAddress.ScriptPubKey);
// Send the change back
transaction.Outputs.Add(changeAmount, publicKey.ScriptPubKey);


// Get it from the public address
var address = BitcoinAddress.Create("mkZzCmjAarnB31n5Ke6EZPbH64Cxexp3Jp", Network.TestNet);
transaction.Inputs[0].ScriptSig = address.ScriptPubKey;

// OR we can also use the private key 
var bitcoinPrivateKey = new BitcoinSecret("cScHSaYbJfZchsqc6GQqpYv8eiu6t5DGFvz9g92wqaCwVbNJ2W67", Network.TestNet);
transaction.Inputs[0].ScriptSig = publicKey.ScriptPubKey; 

transaction.Sign(bitcoinPrivateKey, receivedCoins.ToArray());

BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

if (!broadcastResponse.Success)
{
    Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
    Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
}
else
{
    Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
    Console.WriteLine(transaction.GetHash());
}

//// Create a client
//QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);
//// Parse transaction id to NBitcoin.uint256 so the client can eat it
//var transactionId = uint256.Parse("b2c9da5658926204043bcbdd98d623b982e5267b6876d9a892d21ddfbf20c2e4");
//// Query the transaction
//GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
//NBitcoin.Transaction transaction = transactionResponse.Transaction;

//List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
//foreach (var coin in receivedCoins)
//{
//    Money amount = (Money)coin.Amount;

//    Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
//    var paymentScript = coin.TxOut.ScriptPubKey;
//    Console.WriteLine(paymentScript);  // It's the ScriptPubKey
//    var address = paymentScript.GetDestinationAddress(Network.Main);
//    Console.WriteLine(address); // 1HfbwN6Lvma9eDsv7mdwp529tgiyfNr7jc
//    Console.WriteLine();
//}

//var outputs = transaction.Outputs;
//foreach (TxOut output in outputs)
//{
//    Money amount = output.Value;

//    Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
//    var paymentScript = output.ScriptPubKey;
//    Console.WriteLine(paymentScript);  // It's the ScriptPubKey
//    var address = paymentScript.GetDestinationAddress(Network.Main);
//    Console.WriteLine(address);
//    Console.WriteLine();
//}

//var inputs = transaction.Inputs;
//foreach (TxIn input in inputs)
//{
//    OutPoint previousOutpoint = input.PrevOut;
//    Console.WriteLine(previousOutpoint.Hash); // hash of prev tx
//    Console.WriteLine(previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
//    Console.WriteLine();
//}

var a = 2;
Console.ReadLine();

