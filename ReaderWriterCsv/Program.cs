using ReaderWriterCsv;

var currentDirectory = CurrentDirectory.Get();
var fileName = "test";

var filePath = currentDirectory + @$"\{fileName}.csv";
var filePathOut = currentDirectory + @$"\{fileName}Out.csv";

var inDatas = ReaderWriterCsv.ReaderWriterCsv.ReadFromCsv<InData>(filePath, ';');

ReaderWriterCsv.ReaderWriterCsv.SaveToCsv<InData>(inDatas, filePathOut, ';');


