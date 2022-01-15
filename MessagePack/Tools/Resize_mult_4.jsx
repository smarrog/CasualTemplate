doc = app.activeDocument;  

var fWidth = doc.width;
var fHeight = doc.height;

while(fWidth%4!=0){
  fWidth++;
}
while(fHeight%4!=0){
  fHeight++;
}

app.activeDocument.resizeCanvas(UnitValue(fWidth,"px"),UnitValue(fHeight,"px"));

doc.save();
