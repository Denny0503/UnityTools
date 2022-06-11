// C_TopMethods.cpp : 定义 DLL 的导出函数。
//

#include "pch.h"
#include "framework.h"
#include "C_TopMethods.h"


// 这是导出变量的一个示例
CTOPMETHODS_API int nCTopMethods = 0;

// 这是导出函数的一个示例。
CTOPMETHODS_API int fnCTopMethods(void)
{
	return 0;
}

// 这是已导出类的构造函数。
CCTopMethods::CCTopMethods()
{
	return;
}

/// <summary>
/// 枚举本地摄像机列表
/// </summary>
/// <param name="device"></param>
/// <returns></returns>
CTOPMETHODS_API int EnumerateVideoDevice(VideoDevice* device)
{
	vector<DeviceInfo> list;

	int camera = VideoDeviceHelper::EnumerateLocalVideoDevice(list);
	device->DeviceCount = camera;

	if (camera > 0 && camera < 25)
	{
		for (size_t i = 0; i < camera; i++)
		{
			device->VideoDeviceList[i] = list[i];
		}
	}

	return camera;
}
