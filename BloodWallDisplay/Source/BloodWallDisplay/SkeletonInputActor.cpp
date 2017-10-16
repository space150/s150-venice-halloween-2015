// Fill out your copyright notice in the Description page of Project Settings.

#include "SkeletonInputActor.h"


// Sets default values
ASkeletonInputActor::ASkeletonInputActor()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	id = -1;
	leftHand = FVector2D(-1, -1);
	rightHand = FVector2D(-1, -1);
}

// Called when the game starts or when spawned
void ASkeletonInputActor::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void ASkeletonInputActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);


}

