git clone --depth 1 https://github.com/neologd/mecab-unidic-neologd.git
./mecab-ipadic-neologd/bin/install-mecab-unidic-neologd -n -y -u -p $(cd $(dirname $0); pwd)/unidic-neologd
rm -f ./unidic-neologd/*.def
mv ./unidic-neologd ../
rm -r -f ./mecab-ipadic-neologd
