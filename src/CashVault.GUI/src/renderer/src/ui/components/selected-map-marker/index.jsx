/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@src/app/domain/administration/stores'

import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useEffect } from 'react'

const MapSelectedMarker = styled.div`
  position: absolute;
  bottom: 0;
  width: 100%;
  z-index: 19999;
  box-shadow: 0 -10px 10px rgba(0, 0, 0, 0.16);

  & .close-button {
    position: absolute;
    top: -2rem;
    right: 3.5rem;

    & .text-left {
      color: black;
    }

    & svg {
      color: var(--bg-medium);
    }
  }

  & .wave-svg {
    position: absolute;
    top: -4rem;
    right: 0;
    height: 4.2rem;
    width: 100%;
    min-width: 100%;
  }

  & .content {
    padding: 4.188rem 2.5rem 2.5rem 6.25rem;
    background-color: white !important;
    width: 100%;

    & .actions {
      display: flex;
      gap: 2rem;
      margin-top: 4.125rem;
    }

    & .locker-info {
      display: flex;
      gap: 2rem;
      h2 {
        font-size: 3rem;
        font-weight: 700;
        line-height: 3.5rem;
      }

      p {
        font-size: 1.625rem;
        font-weight: 400;
        line-height: 2.375rem;
      }
    }
  }
`
const WaveHeader = () => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      className="wave-svg"
      viewBox="0 0 913.7052631578947 59.07"
      preserveAspectRatio="none"
    >
      <path
        fill="#FFFFFF"
        d="M631.909 60.6565C652.936 60.6565 672.59 49.0723 686.478 31.5263L691.785 24.8097C704.266 9.03716 722.241 0 741.149 0H918V537H0V60.6565H631.909Z"
      />
    </svg>
  )
}

const SelectedMapMarker = (props) => {
  const {
    showCloseButton = true,
    showFooterActions = true,
    selectedMapMarker = {},
    onClose = () => {},
    onAccept = () => {},
    iconUrl,
    customFooterActions
  } = props
  const { t } = useTranslation()

  useEffect(() => {
    console.info('selected map marker, ', selectedMapMarker)
  }, [selectedMapMarker])

  return (
    <MapSelectedMarker>
      <WaveHeader />
      {showCloseButton && (
        <CircleButton
          className="close-button"
          textLeft={t('Close')}
          onClick={onClose}
          size="s"
          icon={(props) => <IconClose {...props} />}
        />
      )}
      <div className="content">
        <div className="locker-info">
          <img src={iconUrl} />
          <div>
            <h2>{`${selectedMapMarker?.postalService} ${selectedMapMarker?.locationType}`}</h2>
            <p>{`${selectedMapMarker?.streetName} ${selectedMapMarker?.streetNumber || ''}, ${selectedMapMarker?.postalCode} ${selectedMapMarker?.city}`}</p>
          </div>
        </div>
        {showFooterActions && (
          <div className="actions">
            {customFooterActions !== null && customFooterActions !== undefined ? (
              customFooterActions
            ) : (
              <>
                <CircleButton
                  size="l"
                  color="medium"
                  icon={(props) => <IconClose {...props} />}
                  textRight={t('Close')}
                  onClick={onClose}
                />
                <CircleButton
                  size="l"
                  icon={(props) => <IconRightHalfArrow {...props} />}
                  textRight={t('Accept')}
                  onClick={onAccept}
                />
              </>
            )}
          </div>
        )}
      </div>
    </MapSelectedMarker>
  )
}

export default SelectedMapMarker
