using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reader.Data.Storage;

public static class ProductStorage
{
    public const string DemoTitle = "Introduction to speed reading.";
    public const string DemoText = "Hello, reader. This in an example text. The following paragraphs illustrate the benefits and potential drawbacks of speed-reading. Speed reading offers several benefits, primarily by enhancing efficiency and comprehension. By increasing reading speed, individuals can cover more material in less time, allowing them to absorb information faster and improving productivity. This skill is particularly advantageous for students, professionals, and anyone with a large volume of reading material to process. Additionally, speed reading can enhance memory retention and cognitive abilities, as it encourages the brain to process information more rapidly and efficiently. \n\nMoreover, speed reading techniques often involve strategies to improve comprehension while reading at a faster pace. These methods include minimizing subvocalization (the habit of silently pronouncing words as they are read), utilizing peripheral vision to capture more words at once, and skimming for key information. By honing these skills, individuals can extract essential concepts from texts more effectively, making them better equipped to analyze and apply the information they encounter. \n\nDespite its benefits, speed reading also comes with potential drawbacks. Critics argue that increasing reading speed at the expense of comprehension may lead to superficial understanding or overlooking nuanced details. Moreover, some studies suggest that the human brain has a natural limit to how fast it can process and comprehend written information, challenging the notion that exponentially increasing reading speed is feasible without sacrificing comprehension. Additionally, speed reading techniques may not be suitable for all types of reading material, such as complex academic texts or literary works that require deep reflection and analysis. Therefore, while speed reading can be a valuable skill, it is essential to balance speed with comprehension to ensure meaningful learning and understanding.";
    public const string DefaultNewTitle = "New Title";
    public const string DefaultNewText = "Text";

    public const string SupportedFileImports = ".pdf, .txt, .md, .html, .epub";
}
