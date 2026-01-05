using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.DeviceDriver.Common.Helpers;

public class ByteHelper
{
    /// <summary>
    /// Sets a specific bit in a byte
    /// </summary>
    /// <param name="b">Byte which is to be processed</param>
    /// <param name="bitIndex">Index</param>
    /// <returns></returns>
    public static byte SetBit(byte b, int bitIndex)
    {
        if (bitIndex < 8 && bitIndex > -1)
        {
            return (byte)(b | (byte)(0x01 << bitIndex));
        }

        throw new InvalidOperationException($"The value for BitNumber {bitIndex} was not in the permissible range! (BitNumber = (min)0 - (max)7)");
    }

    /// <summary>
    /// Get a specific bit in a byte
    /// </summary>
    /// <param name="b">Byte which is to be processed</param>
    /// <param name="bitIndex">Index</param>
    /// <returns></returns>
    public static bool GetBit(byte b, int bitIndex)
    {
        return (b & 1 << bitIndex) != 0;
    }

    /// <summary>
    /// Get all bits in a byte
    /// </summary>
    /// <param name="b">Byte which is to be processed</param>
    /// <returns></returns>
    public static ByteBitInfo GetBits(byte b)
    {
        return new ByteBitInfo
        {
            Bit0 = GetBit(b, 0),
            Bit1 = GetBit(b, 1),
            Bit2 = GetBit(b, 2),
            Bit3 = GetBit(b, 3),
            Bit4 = GetBit(b, 4),
            Bit5 = GetBit(b, 5),
            Bit6 = GetBit(b, 6),
            Bit7 = GetBit(b, 7),
        };
    }

    public static byte[] ConcatenateByteArrays(byte[] array1, byte[] array2)
    {
        byte[] result = new byte[array1.Length + array2.Length];
        Buffer.BlockCopy(array1, 0, result, 0, array1.Length);
        Buffer.BlockCopy(array2, 0, result, array1.Length, array2.Length);
        return result;
    }

    public static bool Equals(byte[] array1, byte[] array2)
    {
        if (array1 == null || array2 == null)
        {
            return false;
        }

        if (array1.Length != array2.Length)
        {
            return false;
        }

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// CRC16 calculation method, based on the polynomial, initial value, final XOR value, and reflection settings.
    /// 
    /// 
    /// //// CRC-16-CCITT (False)
    /// //ushort crcCCITTFalse = ByteHelper.CalculateCrc(data, 0x1021, 0xFFFF, 0x0000, false, false);
    /// 
    /// //// CRC-16-ARC
    /// //ushort crcARC = ByteHelper.CalculateCrc(data, 0x8005, 0x0000, 0x0000, true, true);
    /// 
    /// //// CRC-16-AUG-CCITT
    /// //ushort crcAUGCCITT = ByteHelper.CalculateCrc(data, 0x1021, 0x1D0F, 0x0000, false, false);
    /// 
    /// //// CRC-16-BUYPASS
    /// //ushort crcBUYPASS = ByteHelper.CalculateCrc(data, 0x8005, 0x0000, 0x0000, false, false);
    /// 
    /// //// CRC-16-XMODEM
    /// //ushort crcXMODEM = ByteHelper.CalculateCrc(data, 0x1021, 0x0000, 0x0000, false, false);
    /// 
    /// //// CRC-16-MODBUS
    /// //ushort crcMODBUS = ByteHelper.CalculateCrc(data, 0x8005, 0xFFFF, 0x0000, true, true);
    /// 
    /// //// CRC-16-KERMIT
    /// //ushort crcKERMIT = ByteHelper.CalculateCrc(finalMessage, 0x1021, 0x0000, 0x0000, true, true);
    /// 
    /// //// CRC-16-DNP
    /// //ushort crcDNP = ByteHelper.CalculateCrc(data, 0x3D65, 0x0000, 0xFFFF, true, true);
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="polynomial"></param>
    /// <param name="initialValue"></param>
    /// <param name="finalXorValue"></param>
    /// <param name="reflectIn"></param>
    /// <param name="reflectOut"></param>
    /// <returns></returns>
    public static ushort CalculateCrc(byte[] data, ushort polynomial, ushort initialValue, ushort finalXorValue, bool reflectIn, bool reflectOut)
    {
        ushort crc = initialValue;

        foreach (byte b in data)
        {
            byte currentByte = reflectIn ? ReflectByte(b) : b;
            crc ^= (ushort)(currentByte << 8);

            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) != 0)
                {
                    crc = (ushort)(crc << 1 ^ polynomial);
                }
                else
                {
                    crc <<= 1;
                }
            }
        }

        crc ^= finalXorValue;

        return reflectOut ? ReflectUShort(crc) : crc;
    }

    private static byte ReflectByte(byte b)
    {
        byte reflection = 0;
        for (int i = 0; i < 8; i++)
        {
            reflection |= (byte)((b >> i & 1) << 7 - i);
        }
        return reflection;
    }

    private static ushort ReflectUShort(ushort us)
    {
        ushort reflection = 0;
        for (int i = 0; i < 16; i++)
        {
            reflection |= (ushort)((us >> i & 1) << 15 - i);
        }
        return reflection;
    }
}
