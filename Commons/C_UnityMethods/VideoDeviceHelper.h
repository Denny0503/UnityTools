#pragma once

#ifdef VIDEODEVICEHELPER_EXPORTS
#define VIDEODEVICEHELPER_API __declspec(dllexport)
#else
#define VIDEODEVICEHELPER_API __declspec(dllimport)
#endif

#include <iostream>
#include "strmif.h"
#include <initguid.h>
#include <vector>
#include <string>
#include "dshow.h"

#pragma comment(lib, "setupapi.lib")
#pragma comment(lib, "strmiids.lib")
#pragma comment(lib, "quartz.lib")

using namespace std;

//#define VI_MAX_CAMERAS 20
//DEFINE_GUID(CLSID_SystemDeviceEnum, 0x62be5d10, 0x60eb, 0x11d0, 0xbd, 0x3b, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);
//DEFINE_GUID(CLSID_VideoInputDeviceCategory, 0x860bb310, 0x5d01, 0x11d0, 0xbd, 0x3b, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);
//DEFINE_GUID(IID_ICreateDevEnum, 0x29840822, 0x5b84, 0x11d0, 0xbd, 0x3b, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

using namespace std;

/// <summary>
/// 设备信息
/// </summary>
struct DeviceInfo
{
public:
	/// <summary>
	/// 摄像机序号
	/// </summary>
	int CameraIndex;
	/// <summary>
	/// 摄像机名称
	/// </summary>
	char CameraName[255] = { 0 };
};

/// <summary>
/// 视频设备
/// </summary>
struct VideoDevice
{
	/// <summary>
	/// 设备数目
	/// </summary>
	int DeviceCount;
	/// <summary>
	/// 视频设备列表，最大数目为25
	/// </summary>
	DeviceInfo VideoDeviceList[25];
};

class VIDEODEVICEHELPER_API VideoDeviceHelper
{
public:
	VideoDeviceHelper(void);
	static int EnumerateLocalVideoDevice(vector<DeviceInfo>& list);	
};

