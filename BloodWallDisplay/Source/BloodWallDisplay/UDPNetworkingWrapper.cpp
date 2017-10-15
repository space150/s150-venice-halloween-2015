// Fill out your copyright notice in the Description page of Project Settings.

#include "UDPNetworkingWrapper.h"
#include <string>

UUDPNetworkingWrapper* UUDPNetworkingWrapper::Constructor()
{
	return (UUDPNetworkingWrapper*)StaticConstructObject(UUDPNetworkingWrapper::StaticClass());
}
UUDPNetworkingWrapper* UUDPNetworkingWrapper::ConstructUDPWrapper(const FString& Description, const FString& SenderSocketName, const FString& TheIP, const int32 ThePort, const int32 BufferSize,
	const bool AllowBroadcast, const bool Bound, const bool Reusable, const bool Blocking)
{

	UUDPNetworkingWrapper* wrapper = Constructor();

	wrapper->SocketSubsystem = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM);
	FIPv4Address::Parse(TheIP, wrapper->RemoteAdress);

	wrapper->SenderSocket = FUdpSocketBuilder(TEXT("DATA"))
		.AsNonBlocking()
		.AsReusable()
		.BoundToAddress(FIPv4Address::Any)
		.BoundToPort(ThePort)
		.WithMulticastLoopback();

	return wrapper;
}

bool UUDPNetworkingWrapper::sendMessage(int32 port, FString Message)
{
	FSocket* Socket = FUdpSocketBuilder(TEXT("DATA"))
		.AsNonBlocking()
		.AsReusable()
		.WithMulticastLoopback();
	FString se = Message;
	FIPv4Endpoint re = FIPv4Endpoint(FIPv4Address::LanBroadcast, port);
	bool c = Socket->Connect(*re.ToInternetAddr());
	TCHAR *serializedChar = se.GetCharArray().GetData();
	int32 size = FCString::Strlen(serializedChar);
	int32 sent = 0;
	bool s = Socket->Send((uint8*)TCHAR_TO_UTF8(serializedChar), size, sent);
	Socket->Close();

	return s;
}

FString UUDPNetworkingWrapper::GrabWaitingMessage()
{
	uint32 Size;

	TSharedRef<FInternetAddr> Sender = SocketSubsystem->CreateInternetAddr();

	while (SenderSocket->HasPendingData(Size))
	{
		int32 Read = 0;
		ReceivedData.Init(0, FMath::Min(Size, 65507u));
		SenderSocket->RecvFrom(ReceivedData.GetData(), ReceivedData.Num(), Read, *Sender);
	}


	return StringFromBinaryArray(ReceivedData);

}

bool UUDPNetworkingWrapper::anyMessages()
{

	uint32 Size;

	if (SenderSocket->HasPendingData(Size))
	{
		return true;
	}

	return false;
}

FString UUDPNetworkingWrapper::StringFromBinaryArray(const TArray<uint8>& BinaryArray)
{
	//Create a string from a byte array!
	const std::string cstr(reinterpret_cast<const char*>(BinaryArray.GetData()), BinaryArray.Num());

	//FString can take in the c_str() of a std::string
	return FString(cstr.c_str());

}

void UUDPNetworkingWrapper::UDPDestructor()
{
	SocketSubsystem->DestroySocket(SenderSocket);
	SenderSocket = nullptr;
	SocketSubsystem = nullptr;
}

