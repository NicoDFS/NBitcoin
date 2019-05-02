using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoin.Altcoins.Nist5
{
    public class Nist5
    {
		private IHash[] _hashers;

		public Nist5()
		{
			var blake512 = HashFactory.Crypto.SHA3.CreateBlake512();
			var groestl512 = HashFactory.Crypto.SHA3.CreateGroestl512();
			var skein512 = HashFactory.Crypto.SHA3.CreateSkein512_Custom();
			var jh512 = HashFactory.Crypto.SHA3.CreateJH512();
			var keccak512 = HashFactory.Crypto.SHA3.CreateKeccak512();
			
			_hashers = new IHash[] 
			{
				blake512, groestl512, skein512, jh512, keccak512
				
			};
		}

		public byte[] ComputeBytes(byte[] input)
		{
			byte[] hashResult = input;
			for (int i = 0; i < _hashers.Length; i++)
			{
				hashResult = _hashers[i].ComputeBytes(hashResult).GetBytes();
			}

			return hashResult.Take(32).ToArray();
		}
    }
}
