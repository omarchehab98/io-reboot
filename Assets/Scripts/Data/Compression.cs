using System;
using System.Collections.Generic;
using UnityEngine;

public static class Compression {

	struct Size {
		public int x { get; set; }
		public int y { get; set; }

		public Size(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	public static int[] toBase (int K, int num) {
		List<int> digits = new List<int>();
		int index = 0;
		while (num != 0)
		{
			int remainder = num % K;
			num = (int) (num / K);
			digits.Add(remainder);
		}
		if(digits.Count == 0) {
			digits.Add(0);
		}
		return digits.ToArray();
	}

	public static int fromBase(int K, int[] digits) {
		int num = 0;
		for (int i = 0; i < digits.Length; i++) {
			num += (int) (digits[i] * Math.Pow(K, i));
		}
		return num;
	}

	private static readonly char[] symbols = new char[]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',  'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
	private static char toBase62Symbol(int digit) {
		return symbols[digit];
	}

	private static int fromBase62Symbol(char digit) {
		return Array.IndexOf(symbols, digit);
	}

	public static string toBase62String(int[] digits) {
		string str = "";
		for (int i = 0; i < digits.Length; i++) {
			str += toBase62Symbol(toBase(62, digits[i])[0]);
		}
		return str;
	}

	public static int[] fromBase62String(string str) {
		int[] digits = new int[str.Length];
		for (int i = 0; i < str.Length; i++) {
			digits[i] = fromBase62Symbol(str[i]);
		}
		return digits;
	}


	// cell: a data structure, array of three numbers
	// digits[0]: { 0, 1, 2, 3, 4 }
	// digits[1]: { 0, 1, 2, 3 }
	// digits[2]: { 0, 1, 2, ..., n }
	//
	// This function represents a data structure as an integer
	// Known as combinatorics in math
	private static int[] cellUnshrink(string str) {
		int num = fromBase(62, fromBase62String(str));
		return new int[]{ num % 5, (int) (num / 5) % 4, (int) (num / 5 / 4) };
	}

	private static string cellShrink(int[] cell, int buffer) {
		string str = toBase62String(toBase(62, cell[0] + cell[1] * 5 + cell[2] * 5 * 4));
		if (buffer - str.Length < 0) throw new ArgumentException("Integer overflow, increase buffer size.");
		return str + ((buffer - str.Length > 0) ? new String('0', buffer - str.Length) : "");
	}

	// channels: a data structure, two dimensional array of cells
	public static int[,,] uncompress(string str) {
		Size size = new Size(0, fromBase(62, new int[]{ fromBase62Symbol(str[0]) }));
		size.x = (str.Length - 1) / 2 / size.y;
		int[,,] channels = new int[size.y, size.x, 3];
		for (int i = 1; i < str.Length; i += 2) {
			int[] channel = cellUnshrink(str.Substring(i, 2));
			int x = (i - 1) / 2 % size.x,
				y = (int) ((i - 1) / 2 / size.x);
			channels[y, x, 0] = channel[0];
			channels[y, x, 1] = channel[1];
			channels[y, x, 2] = channel[2];
		}
		return channels;
	}

	public static string compress(int[,,] channels, List<Vector2> balls) {
		Size size = new Size(channels.GetLength(1), channels.GetLength(0));
		string str = toBase(62, size.y)[0].ToString();
		if (size.y >= 62) throw new ArgumentException("Integer overflow, increase the size digit.");
		for (int y = 0; y < size.y; y++) {
			for (int x = 0; x < size.x; x++) {
				str += cellShrink(new int[3]{ channels[y, x, 0], channels[y, x, 1], channels[y, x, 2] }, 2);
			}
		}
		str += "-";
		foreach (Vector2 ball in balls) {
			int x = (int) ball.x;
			int y = (int) ball.y;
			if (x >= 62) throw new ArgumentException("Integer overflow, increase the ball x digit.");
			if (y >= 62) throw new ArgumentException("Integer overflow, increase the ball y digit.");
			str += toBase(62, x)[0].ToString() + toBase(62, y)[0].ToString();
		}
		return str;
	}
}