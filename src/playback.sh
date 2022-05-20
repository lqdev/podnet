echo $1
rootpath=$(realpath $1)
echo "${rootpath}"
for dir in `ls -d ${rootpath}/*`
do
  cd $dir
  for fn in `ls`
  do  
    filepath=$(basename "$fn")
    filename="${filepath%.*}"
    fileext="${filepath##*.}"
    outfilepath="2x_${filename}.${fileext}"
    echo "${filepath} | ${outfilepath}"
    ffmpeg -i ${filepath} -filter:a "atempo=2.0" -vn ${outfilepath}
  done
done