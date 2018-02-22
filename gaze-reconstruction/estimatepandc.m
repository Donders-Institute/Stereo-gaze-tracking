  function [snorm,c,p]=estimatepandc(params,T1,l1,l2,v1,v2,u11,u12,u21,u22)
% Estimate the 3D positions of the center of the virtual pupil and center
% of corneal curvature. See Barsingerhorn, Boonstra & Goossens (2018),
% Behav Res Meth

% Annemiek Barsingerhorn
  


% Based on Guestrin and Eizenman (2008)
% And on Novel Eye Gaze Tracking Techniques Under Natural Head Movement
% Zhiwei Zhu and Qiang Ji 2005

% params is a stereoParameters object
% l1 = location of light source 1 (nodal point first camera = origin camera
% coordinate system
% l2 = location of light source 2
% o1 = nodal point camera 1 -> [0,0,0]
% o2 = nodal point camera 2 -> is translation vector T1
% c = center of corneal curvature
% p = pupil center
% v1 = image pupil center in cam 1
% v2 = image pupil center in cam 2
% u11= image glint 1 in cam 1
% u12= image glint 2 in cam 1
% u21= image glint 1 in cam 2
% u22= image glint 2 in cam 2


% geometric measures based on calibration
o1=[0,0,0]';
o2=T1;

[Xc_1_left,~] = triangulate(undistortPoints(v1',params.CameraParameters2),undistortPoints(v2',params.CameraParameters1),params);

Xc_1_left=Xc_1_left'/10;

% [Xc_1_left,Xc_1_right] = stereo_triangulation(v1,v2,om,T,fc_left,cc_left,kc_left,alpha_c_left,fc_right,cc_right,kc_right,alpha_c_right); 

p=Xc_1_left;

[Xc_1_leftglint1,~] = triangulate(undistortPoints(u11',params.CameraParameters2),undistortPoints(u12',params.CameraParameters1),params);
Xc_1_leftglint1=Xc_1_leftglint1'/10;

[Xc_1_leftglint2,~] = triangulate(undistortPoints(u21',params.CameraParameters2),undistortPoints(u22',params.CameraParameters1),params);
Xc_1_leftglint2=Xc_1_leftglint2'/10;

normglint1=l1-Xc_1_leftglint1;

normglint2=l2-Xc_1_leftglint2;

A1=Xc_1_leftglint1;
A2=Xc_1_leftglint1+normglint1;
B1=Xc_1_leftglint2;
B2=Xc_1_leftglint2+normglint2;
nA = dot(cross(B2-B1,A1-B1),cross(A2-A1,B2-B1));
nB = dot(cross(A2-A1,A1-B1),cross(A2-A1,B2-B1));
d = dot(cross(A2-A1,B2-B1),cross(A2-A1,B2-B1));
A0 = A1 + (nA/d)*(A2-A1);
B0 = B1 + (nB/d)*(B2-B1);
c=(A0+B0)/2;

snorm=(p-c)/norm(p-c);




