using UnityEngine;
using System;
using System.Collections;

public class Chance
{
	public enum Coin { Heads, Tails }
	public enum Color { Red, Black, White, Green }
	public enum PlayingCardSuit { Hearts, Diamonds, Spades, Clubs }
	public enum PlayingCard { HeartsA, HeartsK, HeartsQ, HeartsJ, Hearts10, Hearts9, Hearts8, 
							  Hearts7, Hearts6, Hearts5, Hearts4, Hearts3, Hearts2, 
							  DiamondsA, DiamondsK, DiamondsQ, DiamondsJ, Diamonds10, Diamonds9, Diamonds8, 
							  Diamonds7, Diamonds6, Diamonds5, Diamonds4, Diamonds3, Diamonds2, 
							  SpadesA, SpadesK, SpadesQ, SpadesJ, Spades10, Spades9, Spades8, 
							  Spades7, Spades6, Spades5, Spades4, Spades3, Spades2,
							  ClubsA, ClubsK, ClubsQ, ClubsJ, Clubs10, Clubs9, Clubs8, 
							  Clubs7, Clubs6, Clubs5, Clubs4, Clubs3, Clubs2 }

	static public readonly PlayingCardSuit[] PLAYING_CARD_SUITES = { PlayingCardSuit.Hearts, PlayingCardSuit.Diamonds, PlayingCardSuit.Spades, PlayingCardSuit.Clubs };
	static public readonly PlayingCard[] PLAYING_CARDS = { 
							  PlayingCard.HeartsA, PlayingCard.HeartsK, PlayingCard.HeartsQ, PlayingCard.HeartsJ, PlayingCard.Hearts10, PlayingCard.Hearts9, PlayingCard.Hearts8, 
							  PlayingCard.Hearts7, PlayingCard.Hearts6, PlayingCard.Hearts5, PlayingCard.Hearts4, PlayingCard.Hearts3, PlayingCard.Hearts2, 
							  PlayingCard.DiamondsA, PlayingCard.DiamondsK, PlayingCard.DiamondsQ, PlayingCard.DiamondsJ, PlayingCard.Diamonds10, PlayingCard.Diamonds9, PlayingCard.Diamonds8, 
							  PlayingCard.Diamonds7, PlayingCard.Diamonds6, PlayingCard.Diamonds5, PlayingCard.Diamonds4, PlayingCard.Diamonds3, PlayingCard.Diamonds2, 
							  PlayingCard.SpadesA, PlayingCard.SpadesK, PlayingCard.SpadesQ, PlayingCard.SpadesJ, PlayingCard.Spades10, PlayingCard.Spades9, PlayingCard.Spades8, 
							  PlayingCard.Spades7, PlayingCard.Spades6, PlayingCard.Spades5, PlayingCard.Spades4, PlayingCard.Spades3, PlayingCard.Spades2,
							  PlayingCard.ClubsA, PlayingCard.ClubsK, PlayingCard.ClubsQ, PlayingCard.ClubsJ, PlayingCard.Clubs10, PlayingCard.Clubs9, PlayingCard.Clubs8, 
							  PlayingCard.Clubs7, PlayingCard.Clubs6, PlayingCard.Clubs5, PlayingCard.Clubs4, PlayingCard.Clubs3, PlayingCard.Clubs2 };
	
	/** Returns the index of an array according to roll chances. */
	static public int Roll( uint[] chances )
	{
		int len = chances.Length;

		uint[] sortedChances = new uint[len];
		uint[] sortedRanges = new uint[len + 1];

		Array.Copy( chances, sortedChances, len );
		Array.Sort( sortedChances );

		uint totalPercent = 0;
		sortedRanges[0] = 0;

		for ( int i = 0; i < len; i++ )
		{
			uint chance = sortedChances[i];
			totalPercent += chance;
			sortedRanges[i + 1] = totalPercent;
		}

		int roll = UnityEngine.Random.Range( 0, (int)totalPercent );
		int rollIndex = 0;

		for ( int i = 0; i < len; i++ )
		{
			uint min = sortedRanges[i];
			uint max = sortedRanges[i + 1];

			if ( roll >= min && roll <= max )
			{
				uint chance = max - min;
				rollIndex = Array.IndexOf<uint>( chances, chance );
				break;
			}
		}

		return rollIndex;
	}
	
	/** Returns an integer from a standard die (values 1 through 6). */
	static public int DiceRoll() 
	{
		return UnityEngine.Random.Range( 1, 7 );
	}
	
	/** Returns the int rolls of a given amount of standard die (values 1 through 6). */
	static public int DiceRoll( int amountOfDie ) 
	{
		int roll = 0;
		while ( --amountOfDie > 0 ) roll += DiceRoll();
		return roll;
	}
	
	/** Returns an integer ranging from the die values 1 through n. */
	static public int NSidedDiceRoll( int n ) 
	{
		return UnityEngine.Random.Range( 1, n + 1 );
	}
	
	/** Returns the added rolls of x amount of die with values 1 through n. */
	static public int NSidedDiceRoll( int n, int amountOfDie ) 
	{
		int roll = 0;
		while ( --amountOfDie > 0 ) roll += NSidedDiceRoll( n );
		return roll;
	}
	
	/** Returns an integer from a custom die with preset values (e.g. those used in RPG board games). */
	static public int CustomDiceRoll( int[] values ) 
	{
		return values[UnityEngine.Random.Range( 0, values.Length )];
	}
	
	/** Returns the added rolls of x amount of custom die with preset values (e.g. those used in RPG board games). */
	static public int CustomDiceRoll( int[] values, int amountOfDie ) 
	{
		int roll = 0;
		while ( --amountOfDie > 0 ) roll += CustomDiceRoll( values );
		return roll;
	}
	
	/** Returns "Heads" or "Tails" from a coin flip. */
	static public Coin FlipCoin() 
	{
		return (UnityEngine.Random.Range( 0, 2 ) == 1) ? Coin.Heads : Coin.Tails;
	}
	
	/** Returns "Red" or "Black" (referring to the common gambling colors). */
	static public Color FlipRedBlack() 
	{
		return (UnityEngine.Random.Range( 0, 2 ) == 1) ? Color.Red : Color.Black;
	}
	
	/** Returns a random suite of poker playing cards ("Hearts", "Diamonds", "Spades", "Clubs"). */
	static public PlayingCardSuit RandomSuite() 
	{
		return PLAYING_CARD_SUITES[UnityEngine.Random.Range( 0, 4 )];
	}
	
	/** Returns a random card from a standard poker 52-card deck. */
	static public PlayingCard RandomPlayingCard() 
	{
		return PLAYING_CARDS[UnityEngine.Random.Range( 0, 52 )];
	}

}
