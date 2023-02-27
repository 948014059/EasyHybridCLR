import pandas as pd
import argparse
import os
import glob


def Excel2Csv(excel,csvSvaePath):
    excel.to_csv(csvSvaePath,index=None,header=None)

def Excel2Csharp(strList,csharpSavePath,csharpClssName):
    with open(csharpSavePath,"w" ,encoding="utf-8") as f:
        f.write("using UnityEngine;\nusing System.Collections;\nusing System;\nusing System.Collections.Generic;\n")
        f.write("\n")
        f.write("public class %s : BaseTable\n"%csharpClssName)
        f.write("{\n")
        for item in strList:
            f.write("   %s\n"%item)
        f.write("\n")
        f.write("   public List<%s> data = new List<%s>();\n"%(csharpClssName,csharpClssName))
        f.write("\n")
        f.write("   public %s GetDataByID(int id)\n"%csharpClssName)
        f.write("   {\n")
        f.write("       foreach (var item in data)\n")
        f.write("       {\n")
        f.write("           if (item.id == id)\n")
        f.write("           {\n")
        f.write("               return item;\n")
        f.write("           }\n")
        f.write("       }\n")
        f.write("       return null;\n")
        f.write("   }\n")
        f.write("\n")
        f.write("   public override string GetTablePath()\n")
        f.write("   {\n")
        f.write("       return \"%s\";\n"%("ExcelData/"+csharpClssName))
        f.write("   }\n")
        f.write("}\n")

parser = argparse.ArgumentParser(description="manual to this script")
parser.add_argument("--excelpath",type=str,default=None)
parser.add_argument("--cspath",type=str,default=None)
parser.add_argument("--txtpath",type=str,default=None)
parser.add_argument("--extension",type=str,default="xls")
args = parser.parse_args()

if __name__ == "__main__":

    excelFloderPath = args.excelpath
    csSavePath = args.cspath
    txtSavePath = args.txtpath
    extensionStr = args.extension

    if os.path.isdir(excelFloderPath):
        excelFilePaths = glob.glob(excelFloderPath+"/*.%s"%extensionStr)
        for excelfilePath in excelFilePaths:
            _excelPath = excelfilePath.replace("\\","/")
            _csharpClssName = _excelPath.split("/")[-1].split(".")[0]
            print("正在转换：",_csharpClssName)
            _txtSvaePath = os.path.join(txtSavePath,_csharpClssName+".txt").replace("\\","/") 
            _csharpSavePath = os.path.join(csSavePath,_csharpClssName+".cs").replace("\\","/") 
            ExcelDAta = pd.read_excel(_excelPath,header=None)
            rowLineLength = len(ExcelDAta.iloc[0])
            variableStrList =[]
            for i in range(rowLineLength):
                Datatype =  ExcelDAta.iloc[0][i]
                summary  =  ExcelDAta.iloc[1][i]
                DataName =  ExcelDAta.iloc[2][i]
                variableStrList.append("\n    /// <summary>\n    /// %s\n    /// </summary>\n"%summary+
                    "   public %s %s;"%(Datatype,DataName)) 
            Excel2Csv(ExcelDAta[2:],_txtSvaePath)
            Excel2Csharp(variableStrList,_csharpSavePath,_csharpClssName)

print("转换已完成")




