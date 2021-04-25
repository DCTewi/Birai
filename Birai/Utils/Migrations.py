#coding=utf-8

import requests
import time

class Video:
    def __init__(self, bvid, tname, copyright, title, duration) -> None:
        self.bvid = bvid
        self.tname = tname
        self.copyright = copyright
        self.title = title
        self.duration = duration

    def __eq__(self, o: object) -> bool:
        return o.bvid == self.bvid

def getVideoInfo(bvid : str) -> Video:
    url = "https://api.bilibili.com/x/web-interface/view?bvid={}".format(bvid.strip())
    jsonraw = requests.request("GET", url).json()

    jsondata = jsonraw['data']
    tname = jsondata['tname']
    copyright = jsondata['copyright']
    title = jsondata['title']
    duration = jsondata['duration']

    time.sleep(0.2)
    return Video(bvid, tname, copyright, title, duration)

def getOldVersionCSV():
    with open("videos.csv", encoding="utf-8") as f:
        return f.read()

def migrate():
    oldverCsv = getOldVersionCSV()
    lines = oldverCsv.splitlines()
    for line in lines:
        lineSlice = line.split(',')
        oldurl = lineSlice[1].split('/')
        vinfo = getVideoInfo(oldurl[len(oldurl) - 1])

        newline = "https://bilibili.com/{},{},{},{}:{},{},{},{}\n".format(
            vinfo.bvid,
            vinfo.title.strip().replace(',', ' ').replace('\n', ''),
            vinfo.tname,
            vinfo.duration // 60,
            vinfo.duration % 60,
            "原创" if vinfo.copyright == 1 else "搬运",
            lineSlice[0].strip(),
            lineSlice[2].strip().replace(',', ' ').replace('\n', ' ')
        )

        with open("output.csv", mode="a", encoding="utf-8") as f:
            f.write(newline)

migrate()

