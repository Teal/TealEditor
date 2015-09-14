﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor.Test {
    class DocumentLineTest {

        void d() {
            var s = new string[3];

            var a = 0;
            foreach(var i in s) {
                s[a++] = "ads";
                Console.WriteLine(s?.Length ?? 0);
            }

        }

        public void run() {

            var multiComment = new MultiLineBlockSegmentType(new CaseSensitiveStringPettern("/*"), new CaseSensitiveStringPettern("*/"), null);
            var signleComment = new SingleLineBlockSegmentType(new CaseSensitiveStringPettern("//"), new AnyDismatchPettern(), null);
            var keyword_if = new WordSegmentType(new CaseSensitiveStringPettern("if"), null);
            var keyword_else = new WordSegmentType(new CaseSensitiveStringPettern("else"), null);
            var keyword_for = new WordSegmentType(new CaseSensitiveStringPettern("for"), null);
            var str = new SingleLineBlockSegmentType(new CaseSensitiveStringPettern("'"), new CaseSensitiveStringPettern("'"), new SegmentType[] { keyword_if, keyword_else });

            var allChildren = new SegmentType[5]{
                multiComment,
                signleComment,
                str,
                keyword_for,
                null
            };

            var block = new MultiLineBlockSegmentType(new CaseSensitiveStringPettern("{"), new CaseSensitiveStringPettern("}"), allChildren);

            allChildren[4] = block;

            var all = new MultiLineBlockSegmentType(new AnyMatchPettern(), new AnyDismatchPettern(), allChildren);


            var preLine = new DocumentLine("a//b");
            var line = new DocumentLine("a'b if'c");

            line.parseSegments(new BlockSegment(null, all, preLine));

            line.segments.All((a) => {
                Console.WriteLine($"{line[a.startIndex, a.endIndex]}: {a.type}");
                return true;
            });

        }

    }
}