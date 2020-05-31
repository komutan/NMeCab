if [ -d unidic-2.1.2 ];then
  echo "unidic-2.1.2 already exists."
  exit 0
fi

if [ ! -e unidic-mecab-2.1.2_bin.zip ];then
  wget https://unidic.ninjal.ac.jp/unidic_archive/cwj/2.1.2/unidic-mecab-2.1.2_bin.zip
fi

unzip -j unidic-mecab-2.1.2_bin.zip -d ../unidic-2.1.2

rm -i unidic-mecab-2.1.2_bin.zip
