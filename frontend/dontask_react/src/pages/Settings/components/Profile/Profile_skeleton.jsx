import React from "react";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import "./Profile.css";

const ProfileSkeleton = () => {
  return (
    <div className="profile-container">
      <div className="profile-header-group">
        <div className="profile-avatar-container">
          <Skeleton circle={true} height={240} width={240} />
          <div className="load-avatar-button">
            <Skeleton height={65} width={65} style={{ borderRadius: "1rem" }} />
          </div>
        </div>
        <div className="profile-button-group">
          <Skeleton
            height={48}
            width={160}
            style={{ borderRadius: "1.5rem" }}
          />
          <Skeleton
            height={48}
            width={160}
            style={{ borderRadius: "1.5rem" }}
          />
        </div>
      </div>
      <div className="profile-info-group">
        <div className="input-group floating-label-group">
          <Skeleton
            height={64}
            width={560}
            style={{ borderRadius: "1.5rem" }}
          />
        </div>
        <div className="input-group floating-label-group">
          <Skeleton
            height={64}
            width={560}
            style={{ borderRadius: "1.5rem" }}
          />
        </div>
        <div className="profile-actions-row">
          <div className="action-buttons-group">
            <Skeleton
              height={48}
              width={160}
              style={{ borderRadius: "1.5rem" }}
            />
            <Skeleton
              height={48}
              width={160}
              style={{ borderRadius: "1.5rem" }}
            />
            <Skeleton
              height={48}
              width={160}
              style={{ borderRadius: "1.5rem" }}
            />
          </div>
          <span className="info-label">
            <Skeleton height={24} width={80} />
          </span>
        </div>
        <div className="input-group floating-label-group invite-input-group">
          <Skeleton
            height={64}
            width={560}
            style={{ borderRadius: "1.5rem" }}
          />
          <div className="button-light-style">
            <Skeleton
              height={60}
              width={60}
              style={{ borderRadius: "1.5rem" }}
            />
          </div>
        </div>
        <div className="action-buttons-group">
          <Skeleton
            height={48}
            width={160}
            style={{ borderRadius: "1.5rem" }}
          />
          <Skeleton
            height={48}
            width={160}
            style={{ borderRadius: "1.5rem" }}
          />
          <Skeleton
            height={48}
            width={160}
            style={{ borderRadius: "1.5rem" }}
          />
        </div>
      </div>
    </div>
  );
};

export default ProfileSkeleton;
