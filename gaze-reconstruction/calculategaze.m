function [pcorrect,ccorrect,normcorrect,ax1,ax2,cgaze,gaze,lengthn ]=calculategaze(c,p,Rpos,Centerscreen,Kcorrect,alpha,beta)
% Calculate the gaze direction based on the position of the pupil and the
% center of curvature and the location and orietation of the screen
%See Barsingerhorn, Boonstra & Goossens (2018), Behav Res Meth

% Annemiek Barsingerhorn
  

% Correct z component
% see Chen et al 2008
ccorrect=c;
pcorrect=p;




for i=1:length(pcorrect)
    lengthn(i)=norm(pcorrect(:,i)-ccorrect(:,i));
end

% lnt=medfilt1(lengthn,50);
lnt=lengthn;

for i=1:length(pcorrect)
    pcorrect(3,i) = real(ccorrect(3,i) - sqrt(Kcorrect^2-(ccorrect(1,i)-pcorrect(1,i))^2-(ccorrect(2,i)-pcorrect(2,i))^2));

 
end

scdistz=Centerscreen(3);
scdistx=Centerscreen(1);
scdisty=Centerscreen(2);

ccorrect(1,:)=ccorrect(1,:)-scdistx;
ccorrect(2,:)=ccorrect(2,:)-scdisty;
ccorrect(3,:)=ccorrect(3,:)-scdistz;

pcorrect(1,:)=pcorrect(1,:)-scdistx;
pcorrect(2,:)=pcorrect(2,:)-scdisty;
pcorrect(3,:)=pcorrect(3,:)-scdistz;

ccorrect=Rpos*ccorrect;
pcorrect=Rpos*pcorrect;

diffs=pcorrect-ccorrect;

ccorrect(1,:)=medfilt1(ccorrect(1,:),20);
ccorrect(2,:)=medfilt1(ccorrect(2,:),20);
ccorrect(3,:)=medfilt1(ccorrect(3,:),20);

pcorrect(1,:)=ccorrect(1,:)+medfilt1(diffs(1,:),20);
pcorrect(2,:)=ccorrect(2,:)+medfilt1(diffs(2,:),20);
pcorrect(3,:)=ccorrect(3,:)+medfilt1(diffs(3,:),20);

for ll=1:length(ccorrect)
   
    normcorrect(ll,:)=(real(pcorrect(:,ll))-real(ccorrect(:,ll)))/norm(real(pcorrect(:,ll))-real(ccorrect(:,ll)));
    
    
end


ax2=real(asind(normcorrect(:,2)));
ax1=real(asind(normcorrect(:,1)./cosd(ax2)));



%%

cgaze=ccorrect;

for i=1:length(cgaze)
nn=cgaze(3,i)/(cosd(ax2(i)+median(beta))*cosd(ax1(i)+median(alpha)));
gaze(i,:)=cgaze(:,i)+nn*[cosd(ax2(i)+median(beta))*sind(ax1(i)+median(alpha)); sind(ax2(i)+median(beta));-cosd(ax2(i)+median(beta))*cosd(ax1(i)+median(alpha)) ];
end
