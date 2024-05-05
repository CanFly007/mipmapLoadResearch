echo Running YaTCompress with bc 1
YaTCompress bc 1 ori.png bc1.dds
echo Done.

echo Running YaTCompress with bc 3
YaTCompress bc 3 ori.png bc3.dds
echo Done.

echo Running YaTCompress with bc 7
YaTCompress bc 7 ori.png bc7.dds
echo Done.

echo Running YaTCompress with bc 1 and mipmap
YaTCompress bc 1 ori.png bc1_mipmap.dds -mipmap
echo Done.

echo Running YaTCompress with bc 3 and mipmap
YaTCompress bc 3 ori.png bc3_mipmap.dds -mipmap
echo Done.

echo Running YaTCompress with bc 7 and mipmap
YaTCompress bc 7 ori.png bc7_mipmap.dds -mipmap
echo Done.

echo Running YaTCompress with astc 4x4
YaTCompress astc 4x4 ori.png 4x4.astc
echo Done.

echo Running YaTCompress with astc 5x5
YaTCompress astc 5x5 ori.png 5x5.astc
echo Done.

echo Running YaTCompress with astc 6x6
YaTCompress astc 6x6 ori.png 6x6.astc
echo Done.

echo All commands executed successfully.
pause