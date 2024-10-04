#ifndef IR_THREAD_H
#define IR_THREAD_H

#include <QtGui>
#include <QThread>
#include <SDL/SDL.h>
#include <vector>
#include <boost/shared_ptr.hpp>

class ContentWindowInterface;
class DisplayGroupIRController;

struck IRControllerState
{
	IRControllerState()
	{
		reset();
	}

	void reset()
	{
		supported = true;
		button1 = 0;
		clickedWindow = boost::shared_ptr<ContentWindowInterface>();
		resizing = false;
	}

	// IRController name
	QString name;

	// whether this IRController is supported or not
	bool supported;

	// cursor click
	int button1;

	// currently clicked window 
	boost::shared_ptr<ContentWindowInterface> clickedWindow;

	// resizing status of window 
	bool resizing;
};

class IRThread : public QThread 
{
	Q_OBJECT

	public:

		IRControllerThread();
		~IRControllerThread();

	protected:
		
		void run();

	public slots:

		void updateIRController();

	private:

		QTimer timer_;

		int tick1_;
		int tick2_;

		/*
		std::vector<SDL_Joystick*> joysticks_;
		std::vector<boost::shared_ptr<DisplayGroupJoystick> > displayGroupJoysticks_;
		std::vector<JoystickState> states_;

		void joystickMoveMarker(int index, float dx, float dy);
		void joystickPan(int index, float dx, float dy);
		void joystickZoom(int index, int dir);
		void joystickScaleSize(int index, int dir);
		*/
};

#endif