using System;
namespace Landmark.ER301Driver.Abstract
{
	public interface ITagDriver
	{
		void Start(string portName);

        void DetectionLoop();
    }
}

