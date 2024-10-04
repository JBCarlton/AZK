// ManagedDll.h

#pragma once

using namespace System;
using namespace System::Reflection;

//namespace HeaderNameSpace 
namespace IRControllerParser
{

    //public ref class HeaderClass
    public ref class KinectParser
    {
        //public:void CSharpMethodCaller(int* argument)
        public:void Build()
        {
            //CSharpNameSpace::CSharpClassName::CSharpMethod(*argument);
            IRController::AzureKinect::AzureKinect();            
            return;
        }
        public:void  Execute()
        {
            IRController::AzureKinect::Execute(); 
            return;
        }
    };
}

private 

__declspec(dllexport) void kinectBuilder()
{
    IRControllerParser::KinectParser kinect;
    kinect.Build();
}
