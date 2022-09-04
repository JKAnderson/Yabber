﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulsFormats
{
    public partial class FLVER2
    {
        /// <summary>
        /// Determines which properties of a vertex are read and written, and in what order and format.
        /// </summary>
        public class BufferLayout : List<FLVER.LayoutMember>
        {
            /// <summary>
            /// The total size of all ValueTypes in this layout.
            /// </summary>
            public int Size => this.Sum(member => member.Size);

            /// <summary>
            /// Creates a new empty BufferLayout.
            /// </summary>
            public BufferLayout() : base() { }

            internal BufferLayout(BinaryReaderEx br) : base()
            {
                int memberCount = br.ReadInt32();
                br.AssertInt32(0);
                br.AssertInt32(0);
                int memberOffset = br.ReadInt32();

                br.StepIn(memberOffset);
                {
                    int structOffset = 0;
                    Capacity = memberCount;
                    for (int i = 0; i < memberCount; i++)
                    {
                        var member = new FLVER.LayoutMember(br, structOffset);
                        structOffset += member.Size;
                        Add(member);
                    }
                }
                br.StepOut();
            }

            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt32(Count);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.ReserveInt32($"VertexStructLayout{index}");
            }

            internal void WriteMembers(BinaryWriterEx bw, int index)
            {
                bw.FillInt32($"VertexStructLayout{index}", (int)bw.Position);
                int structOffset = 0;
                foreach (FLVER.LayoutMember member in this)
                {
                    member.Write(bw, structOffset);
                    structOffset += member.Size;
                }
            }
        }
    }
}
