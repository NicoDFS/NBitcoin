using System;
using System.Linq;
using System.Text;
using NBitcoin.Altcoins.Nist5;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;

namespace NBitcoin.Altcoins
{
	// https://github.com/VulcanoCrypto/Vulcano/blob/master/src/chainparams.cpp
	public class Vulcano : NetworkSetBase
	{
		public static Vulcano Instance { get; } = new Vulcano();

		public override string CryptoCode => "VULC";

		private Vulcano()
		{
		}

		public class VulcanoConsensusFactory : ConsensusFactory
		{
			private VulcanoConsensusFactory()
			{
			}

			public static VulcanoConsensusFactory Instance { get; } = new VulcanoConsensusFactory();

			public override BlockHeader CreateBlockHeader()
			{
				return new VulcanoBlockHeader();
			}
			public override Block CreateBlock()
			{
				return new VulcanoBlock(new VulcanoBlockHeader());
			}
		}


#pragma warning disable CS0618 // Type or member is obsolete
		public class VulcanoBlockHeader : BlockHeader
		{
			// blob
			private static byte[] CalculateHash(byte[] data, int offset, int count)
			{
				return new Nist5.Nist5().ComputeBytes(data.Skip(offset).Take(count).ToArray());
			}

			protected override HashStreamBase CreateHashStream()
			{
				return BufferedHashStream.CreateFrom(CalculateHash);
			}
		}

		public class VulcanoBlock : Block
		{
#pragma warning disable CS0612 // Type or member is obsolete
			public VulcanoBlock(VulcanoBlockHeader h) : base(h)
#pragma warning restore CS0612 // Type or member is obsolete
			{

			}
			public override ConsensusFactory GetConsensusFactory()
			{
				return Instance.Mainnet.Consensus.ConsensusFactory;
			}
		}
#pragma warning restore CS0618 // Type or member is obsolete

		protected override void PostInit()
		{
			RegisterDefaultCookiePath("Vulcano", new FolderName() { TestnetFolder = "testnet4" });
		}

		static uint256 GetPoWHash(BlockHeader header)
		{
			var headerBytes = header.ToBytes();
			var h = NBitcoin.Crypto.SCrypt.ComputeDerivedKey(headerBytes, headerBytes, 1024, 1, 1, null, 32);
			return new uint256(h);
		}

		protected override NetworkBuilder CreateMainnet()
		{
			var builder = new NetworkBuilder();
			builder.SetConsensus(new Consensus()
			{
				//SubsidyHalvingInterval = 700800, #Need to get this
				MajorityEnforceBlockUpgrade = 750,
				MajorityRejectBlockOutdated = 950,
				MajorityWindow = 1000,
				//BIP34Hash = new uint256("0x00000012f1c40ff12a9e6b0e9076fe4fa7ad27012e256a5ad7bcb80dc02c0409"),
				PowLimit = new Target(new uint256("0x00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
				MinimumChainWork = new uint256("0x000002c5551b617c4f02f6be4aa8a03e9a17fec08524bf4cf97fc6d9be5a856e"),
				PowTargetTimespan = TimeSpan.FromSeconds(90),
				PowTargetSpacing = TimeSpan.FromSeconds(90),
				PowAllowMinDifficultyBlocks = false,
				CoinbaseMaturity = 66,
				PowNoRetargeting = false,
				//RuleChangeActivationThreshold = 10752,
				//MinerConfirmationWindow = 13440,
				ConsensusFactory = VulcanoConsensusFactory.Instance,
				//SupportSegwit = true
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 1, 70 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 1, 18 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 28 + 212 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x02, 0x2D, 0x25, 0x33 })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x02, 0x21, 0x31, 0x2B })
			.SetMagic(0x17010208)
			.SetPort(62543)
			.SetRPCPort(62541)
			//.SetMaxP2PVersion(70015)
			.SetName("vulcano-main")
			.AddAlias("vulcano-mainnet")
			.AddDNSSeeds(new[]
			{
				new DNSSeedData("vulcseed1.vulcanocrypto.com", "vulcseed1.vulcanocrypto.com"),
				new DNSSeedData("vulcseed2.vulcanocrypto.com", "vulcseed2.vulcanocrypto.com"),
				new DNSSeedData("vulcseed3.vulcanocrypto.com", "vulcseed3.vulcanocrypto.com"),
				new DNSSeedData("vulcseed4.vulcanocrypto.com", "vulcseed4.vulcanocrypto.com"),
				new DNSSeedData("vulcseed5.vulcanocrypto.com", "vulcseed.vulcanocrypto.com"),
				new DNSSeedData("vulcseed1.vulcano.io", "vulcseed1.vulcano.io"),
				new DNSSeedData("vulcseed2.vulcano.io", "vulcseed2.vulcano.io"),
				new DNSSeedData("vulcseed3.vulcano.io", "vulcseed3.vulcano.io"),
				new DNSSeedData("vulcseed4.vulcano.io", "vulcseed4.vulcano.io"),
				new DNSSeedData("vulcseed5.vulcano.io", "vulcseed5.vulcano.io"),

			})
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("0100000000000000000000000000000000000000000000000000000000000000000000004ff6992938fc1f45d4131b87968d18255ec15b523739ac6340c893d56b6d9777e20d5a5bffff0f1e8f1e20000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff6a04ffff001d01044c614e6f76656d6265722033302032303137202d204e6967657220417070726f7665732041726d656420552e532e2044726f6e6520466c69676874732c20457870616e64696e672050656e7461676f6ee280997320526f6c6520696e20416672696361ffffffff0100f2052a01000000434104243e8da79e117dba99d89a2da6ed761af43175227d19caaffea72398514962af9701478a69410b81");
			return builder;
		}

		protected override NetworkBuilder CreateTestnet()
		{
			var builder = new NetworkBuilder();
			var res = builder.SetConsensus(new Consensus()
			{
				//SubsidyHalvingInterval = 56600,
				MajorityEnforceBlockUpgrade = 51,
				MajorityRejectBlockOutdated = 75,
				MajorityWindow = 100,
				//BIP34Hash = new uint256("0x00000352de593a01e0efcbaec00345ec80d20c7bd2024ec7c2beec048af0e6d9"),
				PowLimit = new Target(new uint256("0x00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
				//MinimumChainWork = new uint256("0x000000000000000000000000000000000000000000000000000000060e06d35d"),
				PowTargetTimespan = TimeSpan.FromSeconds(30),
				PowTargetSpacing = TimeSpan.FromSeconds(30),
				PowAllowMinDifficultyBlocks = true,
				CoinbaseMaturity = 15,
				PowNoRetargeting = false,
				//RuleChangeActivationThreshold = 1512,
				//MinerConfirmationWindow = 2016,
				ConsensusFactory = VulcanoConsensusFactory.Instance,
				//SupportSegwit = true
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 1, 65 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 1, 12 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 1, 239 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x3a, 0x80, 0x61, 0xa0 })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x3a, 0x80, 0x58, 0x37 })
			//.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tchc"))
			//.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tchc"))
			.SetMagic(0xa0f4d9b5)
			.SetPort(62443)
			.SetRPCPort(62441)
			//.SetMaxP2PVersion(70015)
			.SetName("vulcano-test")
			.AddAlias("vulcano-testnet")
			.AddDNSSeeds(new[]
			{
				new DNSSeedData("test-node01.vulcano.io", "test-node01.vulcano.io")
			})
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("0100000000000000000000000000000000000000000000000000000000000000000000004ff6992938fc1f45d4131b87968d18255ec15b523739ac6340c893d56b6d9777e30d5a5bffff0f1e2b201d000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff6a04ffff001d01044c614e6f76656d6265722033302032303137202d204e6967657220417070726f7665732041726d656420552e532e2044726f6e6520466c69676874732c20457870616e64696e672050656e7461676f6ee280997320526f6c6520696e20416672696361ffffffff0100f2052a01000000434104243e8da79e117dba99d89a2da6ed761af43175227d19caaffea72398514962af9701478a69410b81");
			return builder;
		}

		protected override NetworkBuilder CreateRegtest()
		{
			var builder = new NetworkBuilder();
			builder.SetConsensus(new Consensus()
			{
				//SubsidyHalvingInterval = 150,
				MajorityEnforceBlockUpgrade = 51,
				MajorityRejectBlockOutdated = 75,
				MajorityWindow = 100,
				//BIP34Hash = new uint256(),
				PowLimit = new Target(new uint256("0x7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
				MinimumChainWork = new uint256("0x0000000000000000000000000000000000000000000000000000000000000000"),
				PowTargetTimespan = TimeSpan.FromSeconds(24 * 60 * 60),
				PowTargetSpacing = TimeSpan.FromSeconds(1 * 60),
				PowAllowMinDifficultyBlocks = true,
				CoinbaseMaturity = 100,
				PowNoRetargeting = true,
				//RuleChangeActivationThreshold = 108,
				//MinerConfirmationWindow = 144,
				ConsensusFactory = VulcanoConsensusFactory.Instance,
				//SupportSegwit = true
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 1, 65 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 1, 12 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 1, 239 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x3a, 0x80, 0x61, 0xa0 })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x3a, 0x80, 0x58, 0x37 })
			//.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("chcrt"))
			//.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("chcrt"))
			.SetMagic(0xac7ecfa1)
			.SetPort(51476)
			.SetRPCPort(51475)
			//.SetMaxP2PVersion(70015)
			.SetName("volcano-reg")
			.AddAlias("volcano-regtest")
			.AddDNSSeeds(new DNSSeedData[0])
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("0100000000000000000000000000000000000000000000000000000000000000000000004ff6992938fc1f45d4131b87968d18255ec15b523739ac6340c893d56b6d9777e30d5a5bffff0f1e2b201d000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff6a04ffff001d01044c614e6f76656d6265722033302032303137202d204e6967657220417070726f7665732041726d656420552e532e2044726f6e6520466c69676874732c20457870616e64696e672050656e7461676f6ee280997320526f6c6520696e20416672696361ffffffff0100f2052a01000000434104243e8da79e117dba99d89a2da6ed761af43175227d19caaffea72398514962af9701478a69410b81");
			return builder;
		}
	}
}
