using ReaderWriterCsv;

var currentDirectory = CurrentDirectory.Get(4);
const string FileName = "test";

var filePath = currentDirectory + @$"\{FileName}.csv";
var filePathOut = currentDirectory + @$"\{FileName}Out.csv";

var inDatas = ReaderWriterCsv.ReaderWriterCsv.ReadFromCsv<InData>(filePath, ';');

ReaderWriterCsv.ReaderWriterCsv.SaveToCsv<InData>(inDatas, filePathOut, ';', true);


