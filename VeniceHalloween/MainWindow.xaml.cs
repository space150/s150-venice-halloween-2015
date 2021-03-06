﻿//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VeniceHalloween
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using XamlAnimatedGif;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
 
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// definition of bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;

        /// <summary>
        /// Width of display (depth space)
        /// </summary>
        private int displayWidth;

        /// <summary>
        /// Height of display (depth space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// List of colors for each body tracked
        /// </summary>
        private List<Pen> bodyColors;

        /// <summary>
        /// List of costumes, index of costume matches tracked body
        /// </summary>
        private List<CostumeBase> bodyCostumes;

        /// <summary>
        /// List of body states, index of state matches tracked body
        /// </summary>
        private List<BodyState> bodyStates;

        /// <summary>
        /// List of possible weapons
        /// </summary>
        private List<Weapon> weapons;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            // get size of joint space
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            this.bodyCostumes = new List<CostumeBase>();

            this.bodyCostumes.Add(new Dude1Costume());
            this.bodyCostumes.Add(new Chick1Costume());
            this.bodyCostumes.Add(new Dude2Costume());
            this.bodyCostumes.Add(new Chick2Costume());
            this.bodyCostumes.Add(new Dude2Costume());
            this.bodyCostumes.Add(new Chick1Costume());

            this.weapons = new List<Weapon>();

            this.weapons.Add(new Weapon("sword"));
            this.weapons.Add(new Weapon("gun"));
            this.weapons.Add(new Weapon("molotov"));
            this.weapons.Add(new Weapon("crowbar"));
            this.weapons.Add(new Weapon("cat"));
            this.weapons.Add(new Weapon("lightsaber"));
            this.weapons.Add(new Weapon("plunger"));

            this.bodyStates = new List<BodyState>();

            this.bodyStates.Add(new BodyState(this.weapons.Count));
            this.bodyStates.Add(new BodyState(this.weapons.Count));
            this.bodyStates.Add(new BodyState(this.weapons.Count));
            this.bodyStates.Add(new BodyState(this.weapons.Count));
            this.bodyStates.Add(new BodyState(this.weapons.Count));
            this.bodyStates.Add(new BodyState(this.weapons.Count));

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // use the window object as the view model in this simple example
            this.DataContext = this;

            this.giphyDataSource = new GiphyDataSource(Properties.Settings.Default.SearchTerm);

            // initialize the components (controls) of the window
            this.InitializeComponent();
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
        }

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    OnPropertyChanged("StatusText");
                }
            }
        }

        private GiphyDataSource giphyDataSource;

        private string gifUrl;
        public string GifUrl
        {
            get { return this.gifUrl;  }
            set
            {
                if ( this.gifUrl != value )
                {
                    this.gifUrl = value;

                    OnPropertyChanged("GifUrl");
                }
            }
        }

        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }

            this.setupBackgroundImage();
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                    int bodyIndex = 0;
                    foreach (Body body in this.bodies)
                    {
                        Pen drawPen = this.bodyColors[bodyIndex];
                        CostumeBase costume = this.bodyCostumes[bodyIndex];
                        BodyState state = this.bodyStates[bodyIndex];

                        if (body.IsTracked)
                        {
                            this.DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            // convert the joint points to depth (display) space
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            foreach (JointType jointType in joints.Keys)
                            {
                                // sometimes the depth(Z) of an inferred joint may show as negative
                                // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                CameraSpacePoint position = joints[jointType].Position;
                                if (position.Z < 0)
                                {
                                    position.Z = InferredZPositionClamp;
                                }

                                DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }

                            this.bodyStates[bodyIndex].Update(body.HandLeftConfidence, body.HandLeftState, body.HandRightConfidence, body.HandRightState);

                            this.DrawBody(this.bodyStates[bodyIndex], joints, jointPoints, dc, drawPen, costume);

                            this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                        }

                        bodyIndex += 1;
                    }

                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                }
            }
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="bodyState">State of this body (mostly whether it is holding a weapon)</param>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        /// <param name="costume">the costume used to draw this body</param>
        private void DrawBody(BodyState bodyState, IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen, CostumeBase costume)
        {
            try
            {
                this.DrawImageBetweenJoints(costume.Neck, costume.NeckScale, costume.NeckOffset, 0.0f,
                    joints, jointPoints, JointType.Neck, JointType.SpineShoulder, drawingContext, jointPoints[JointType.SpineShoulder]);
                this.DrawImageBetweenJoints(costume.Spine, costume.SpineScale, costume.SpineOffset, 0.0f,
                    joints, jointPoints, JointType.SpineMid, JointType.SpineBase, drawingContext);

                double ribcageScale = this.DrawImageBetweenJoints(costume.Ribcage, costume.RibcageScale, costume.RibcageOffset, 0.0f,
                    joints, jointPoints, JointType.ShoulderLeft, JointType.ShoulderRight, drawingContext, jointPoints[JointType.SpineMid]);

                this.DrawImageBetweenJoints(costume.Head, costume.HeadScale, costume.HeadOffset, 0.0f,
                    joints, jointPoints, JointType.Head, JointType.SpineShoulder, drawingContext);

                this.DrawImageBetweenJoints(costume.BicepRight, costume.BicepScale, costume.BicepOffset, 0.0f,
                    joints, jointPoints, JointType.ShoulderRight, JointType.ElbowRight, drawingContext);
                this.DrawImageBetweenJoints(costume.BicepLeft, costume.BicepScale, costume.BicepOffset, 0.0f,
                    joints, jointPoints, JointType.ShoulderLeft, JointType.ElbowLeft, drawingContext);

                this.DrawImageBetweenJoints(costume.ForearmRight, costume.ForearmScale, costume.ForearmOffset, 0.0f,
                    joints, jointPoints, JointType.ElbowRight, JointType.WristRight, drawingContext);
                this.DrawImageBetweenJoints(costume.ForearmLeft, costume.ForearmScale, costume.ForearmOffset, 0.0f,
                    joints, jointPoints, JointType.ElbowLeft, JointType.WristLeft, drawingContext);

                this.DrawImageBetweenJoints(costume.HandRight, costume.HandScale, costume.HandOffset, 0.0f,
                    joints, jointPoints, JointType.WristRight, JointType.HandTipRight, drawingContext);
                this.DrawImageBetweenJoints(costume.HandLeft, costume.HandScale, costume.HandOffset, 0.0f,
                    joints, jointPoints, JointType.WristLeft, JointType.HandTipLeft, drawingContext);

                this.DrawImageBetweenJoints(costume.Pelvis, costume.PelvisScale, costume.PelvisOffset, 0.0f,
                    joints, jointPoints, JointType.HipLeft, JointType.HipRight, drawingContext, ribcageScale);

                this.DrawImageBetweenJoints(costume.FemurRight, costume.FemurScale, costume.FemurOffset, 0.0f,
                    joints, jointPoints, JointType.HipLeft, JointType.KneeLeft, drawingContext);
                this.DrawImageBetweenJoints(costume.FemurLeft, costume.FemurScale, costume.FemurOffset, 0.0f,
                    joints, jointPoints, JointType.HipRight, JointType.KneeRight, drawingContext);

                this.DrawImageBetweenJoints(costume.ShinRight, costume.ShinScale, costume.ShinOffset, 0.0f,
                    joints, jointPoints, JointType.KneeLeft, JointType.AnkleLeft, drawingContext);
                this.DrawImageBetweenJoints(costume.ShinLeft, costume.ShinScale, costume.ShinOffset, 0.0f,
                    joints, jointPoints, JointType.KneeRight, JointType.AnkleRight, drawingContext);

                this.DrawImageBetweenJoints(costume.FootLeft, costume.FootScale, costume.FootOffset, 0.0f,
                    joints, jointPoints, JointType.AnkleLeft, JointType.FootLeft, drawingContext);
                this.DrawImageBetweenJoints(costume.FootRight, costume.FootScale, costume.FootOffset, 0.0f,
                    joints, jointPoints, JointType.AnkleRight, JointType.FootRight, drawingContext);

                // draw weapons if needed
                if (bodyState.HasLeftHandWeapon)
                {
                    Weapon weapon = this.weapons[bodyState.LeftHandWeaponIndex];
                    this.DrawImageBetweenJoints(weapon.ImageLeft, weapon.ScaleLeft, weapon.OffsetLeft, -90.0f,
                        joints, jointPoints, JointType.HandLeft, JointType.HandTipLeft, drawingContext, jointPoints[JointType.HandLeft], ribcageScale);
                }
                if (bodyState.HasRightHandWeapon)
                {
                    Weapon weapon = this.weapons[bodyState.RightHandWeaponIndex];
                    this.DrawImageBetweenJoints(weapon.ImageRight, weapon.ScaleRight, weapon.OffsetRight, 90.0f,
                        joints, jointPoints, JointType.HandRight, JointType.HandTipRight, drawingContext, jointPoints[JointType.HandRight], ribcageScale);
                }

                this.DrawDebugBody(joints, jointPoints, drawingContext, drawingPen);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private double DrawImageBetweenJoints(ImageSource image, double drawScale, Point centerOffset, double angleOffset, IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return 1.0f;
            }

            double distance = this.calculateDistance(jointPoints[jointType0], jointPoints[jointType1]);
            double scale = distance / image.Height;

            Point center = new Point();
            center.X = (jointPoints[jointType0].X + jointPoints[jointType1].X) / 2;
            center.Y = (jointPoints[jointType0].Y + jointPoints[jointType1].Y) / 2;

            return DrawImageBetweenJoints(image, drawScale, centerOffset, angleOffset, joints, jointPoints, jointType0, jointType1, drawingContext, center, scale);
        }

        private double DrawImageBetweenJoints(ImageSource image, double drawScale, Point centerOffset, double angleOffset, IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Point center)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return 1.0f;
            }

            double distance = this.calculateDistance(jointPoints[jointType0], jointPoints[jointType1]);
            double scale = distance / image.Height;

            return DrawImageBetweenJoints(image, drawScale, centerOffset, angleOffset, joints, jointPoints, jointType0, jointType1, drawingContext, center, scale);
        }

        private double DrawImageBetweenJoints(ImageSource image, double drawScale, Point centerOffset, double angleOffset, IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, double scale)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return scale;
            }

            Point center = new Point();
            center.X = (jointPoints[jointType0].X + jointPoints[jointType1].X) / 2;
            center.Y = (jointPoints[jointType0].Y + jointPoints[jointType1].Y) / 2;

            return DrawImageBetweenJoints(image, drawScale, centerOffset, angleOffset, joints, jointPoints, jointType0, jointType1, drawingContext, center, scale);
        }

        private double DrawImageBetweenJoints(ImageSource image, double drawScale, Point centerOffset, double angleOffset, IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Point center, double scale)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return scale;
            }

            double xDiff = jointPoints[jointType0].X - jointPoints[jointType1].X;
            double yDiff = jointPoints[jointType0].Y - jointPoints[jointType1].Y;
            double angle = Math.Atan2(yDiff, xDiff) * (180.0 / Math.PI) + 90; // needs to be rotated 90 degrees to mesh with WPF transforms

            double ss = scale * drawScale;
            Rect rr = new Rect(-image.Width * 0.5 * ss, -image.Height * 0.5 * ss, image.Width * ss, image.Height * ss);

            drawingContext.PushTransform(new TranslateTransform(center.X+centerOffset.X, center.Y+centerOffset.Y));
            drawingContext.PushTransform(new RotateTransform(angle + angleOffset));
            drawingContext.DrawImage(image, rr);
            drawingContext.Pop();
            drawingContext.Pop();

            return scale;
        }

        private void DrawDebugBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            if (Properties.Settings.Default.DebugDraw)
            {
                // Draw the bones
                foreach (var bone in this.bones)
                {
                    this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
                }

                // Draw the joints
                foreach (JointType jointType in joints.Keys)
                {
                    Brush drawBrush = null;

                    TrackingState trackingState = joints[jointType].TrackingState;

                    if (trackingState == TrackingState.Tracked)
                    {
                        drawBrush = this.trackedJointBrush;
                    }
                    else if (trackingState == TrackingState.Inferred)
                    {
                        drawBrush = this.inferredJointBrush;
                    }

                    if (drawBrush != null)
                    {
                        drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                    }
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            if (Properties.Settings.Default.DebugDraw)
            {
                switch (handState)
                {
                    case HandState.Closed:
                        drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                        break;

                    case HandState.Open:
                        drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                        break;

                    case HandState.Lasso:
                        drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                        break;
                }
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            if (Properties.Settings.Default.DebugDraw)
            {
                FrameEdges clippedEdges = body.ClippedEdges;

                if (clippedEdges.HasFlag(FrameEdges.Bottom))
                {
                    drawingContext.DrawRectangle(
                        Brushes.Red,
                        null,
                        new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
                }

                if (clippedEdges.HasFlag(FrameEdges.Top))
                {
                    drawingContext.DrawRectangle(
                        Brushes.Red,
                        null,
                        new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
                }

                if (clippedEdges.HasFlag(FrameEdges.Left))
                {
                    drawingContext.DrawRectangle(
                        Brushes.Red,
                        null,
                        new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
                }

                if (clippedEdges.HasFlag(FrameEdges.Right))
                {
                    drawingContext.DrawRectangle(
                        Brushes.Red,
                        null,
                        new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
                }
            }
        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

        public double calculateDistance(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        private Animator animator;
        private int iterationCount;

        private void setupBackgroundImage()
        {
            this.iterationCount = 0;
            this.GifUrl = this.giphyDataSource.GetNextGif();
        }
        
        private void backgroundImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (animator != null)
            {
                animator.CurrentFrameChanged -= Animator_CurrentFrameChanged;
            }

            animator = AnimationBehavior.GetAnimator(backgroundImage);
            if (animator != null)
            {
                animator.CurrentFrameChanged += Animator_CurrentFrameChanged;
            }
        }

        private void Animator_CurrentFrameChanged(object sender, EventArgs e)
        {
            if (animator != null)
            {
                if (animator.CurrentFrameIndex >= (animator.FrameCount - 1))
                {
                    this.iterationCount += 1;
                    if ( this.iterationCount >= Properties.Settings.Default.PlaybackIterations)
                    {
                        this.iterationCount = 0;
                        this.GifUrl = this.giphyDataSource.GetNextGif();
                    }
                }
            }
        }

        private void backgroundImage_Error(DependencyObject d, AnimationErrorEventArgs e)
        {
            this.iterationCount = 0;
            this.GifUrl = this.giphyDataSource.GetNextGif();

            System.Diagnostics.Debug.WriteLine($"An error occurred ({e.Kind}): {e.Exception}");
        }
    }
}
