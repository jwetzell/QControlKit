﻿using System.Collections.Generic;

namespace SharpOSC
{
    public static class SlipFrame
    {
        static readonly byte END = 0xc0;
        static readonly byte ESC = 0xdb;
        static readonly byte ESC_END = 0xDC;
        static readonly byte ESC_ESC = 0xDD;

        public static List<byte[]> Decode(byte[] data)
        {
            List<byte[]> messages = new List<byte[]>();

            List<byte> buffer = new List<byte>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == END && buffer.Count > 0)
                {
                    messages.Add(buffer.ToArray());
                    buffer.Clear();
                }
                else if (data[i] != END)
                {
                    buffer.Add(data[i]);
                }
            }
            return messages;
        }

        public static byte[] Encode(byte[] data)
        {
            List<byte> slipData = new List<byte>();

            byte[] esc_end = { ESC, ESC_END };
            byte[] esc_esc = { ESC, ESC_ESC };
            byte[] end = { END };

            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                if (data[i] == END)
                {
                    slipData.AddRange(esc_end);
                }
                else if (data[i] == ESC)
                {
                    slipData.AddRange(esc_esc);
                }
                else
                {
                    slipData.Add(data[i]);
                }
            }
            slipData.AddRange(end);
            return slipData.ToArray();
        }
    }
}