if [ -d unidic-cwj-2.2.0 ];then
  echo "unidic-cwj-2.2.0 already exists."
  exit 0
fi

if [ ! -e unidic-cwj-2.2.0.zip ];then
  wget https://unidic.ninjal.ac.jp/unidic_archive/cwj/2.2.0/unidic-cwj-2.2.0.zip
fi

unzip unidic-cwj-2.2.0.zip -x "*.def" "*.csv" -d ../

rm -i unidic-cwj-2.2.0.zip
