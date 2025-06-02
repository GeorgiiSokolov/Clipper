using System;
using Autodesk.Revit.DB;

namespace Clipper;

// Transaction context, usage:
//
// TrContext.Run(doc, tx =>
// {
//     Do Revit changes here
//     tx is the active Transaction and accessible as a parameter
// }, "Name");

public class TrContext : IDisposable
{
	private readonly Document doc;
	private readonly string name;
	private readonly Transaction transaction;
	private bool isCommitted = false;

	private TrContext(Document doc, string name)
	{
		this.doc = doc;
		this.name = name ?? DateTime.Now.ToString();
		transaction = new Transaction(this.doc, this.name);
		transaction.Start();
	}

	public static void Run(Document doc, Action<Transaction> action, string name = null)
	{
		using (var tx = new TrContext(doc, name))
		{
			try
			{
				action(tx.transaction);
				tx.transaction.Commit();
				tx.isCommitted = true;
			}
			catch
			{
				tx.transaction.RollBack();
				throw; // rethrow to preserve stack trace
			}
		}
	}

	public void Dispose()
	{
		if (!isCommitted && transaction.HasStarted())
		{
			transaction.RollBack();
		}
	}
}