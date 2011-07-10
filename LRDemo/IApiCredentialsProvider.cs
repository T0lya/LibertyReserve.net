using System;
using Magnis.Web.Services.LibertyReserve;

namespace LRDemo
{
	public interface IApiCredentialsProvider
	{
		ApiCredentials Credentials { get; }
		
		bool Validate();
	}
}

