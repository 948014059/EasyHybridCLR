import pandas as pd
import argparse
import os
import glob


def Excel2Csv(excel,csvSvaePath):
    excel.to_csv(csvSvaePath,index=None)

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
        for excelfilePath in excelFilePaths: #文件夹内所有的excel文件
            # 获得名称
            _excelPath = excelfilePath.replace("\\","/")
            _csharpClssName = _excelPath.split("/")[-1].split(".")[0]
            _txtSvaePath = os.path.join(txtSavePath,_csharpClssName+".txt").replace("\\","/") 
            _csharpSavePath = os.path.join(csSavePath,_csharpClssName+".cs").replace("\\","/") 
            #读取excel
            ExcelDAta = pd.read_excel(_excelPath,header=None)
            #读取表头数据 类型  名称  summary
            rowLineLength = len(ExcelDAta.iloc[0])
            variableStrList =[]
            for i in range(1,rowLineLength):
                Datatype =  ExcelDAta.iloc[0][i]
                summary  =  ExcelDAta.iloc[1][i]
                DataName =  ExcelDAta.iloc[2][i]
                variableStrList.append("\n    /// <summary>\n    /// %s\n    /// </summary>\n"%summary+
                     "   public %s %s;"%(Datatype,DataName)) 
            # 创建一个新表，填充可导数据
            newExcelData = []
            for item in ExcelDAta.iloc:
                if item[0] == 1:
                    dataline = []
                    for i in range(1,rowLineLength):
                        # print(item[i]) 
                        dataline.append(item[i])
                    newExcelData.append(dataline)
            print("正在转换：",_csharpClssName,"表头",list(ExcelDAta.iloc[2][1:]))
            newExcel = pd.DataFrame(newExcelData,columns=list(ExcelDAta.iloc[2][1:]))
            Excel2Csv(newExcel,_txtSvaePath)
            Excel2Csharp(variableStrList,_csharpSavePath,_csharpClssName)

print("转换已完成")




