// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "SkeletonInputActor.generated.h"

UCLASS(BlueprintType, Blueprintable)
class BLOODWALLDISPLAY_API ASkeletonInputActor : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ASkeletonInputActor();

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int id;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FVector2D leftHand;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FVector2D rightHand;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	
	
};
