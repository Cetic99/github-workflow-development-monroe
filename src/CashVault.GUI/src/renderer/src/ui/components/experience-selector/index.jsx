/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useRef, useState } from 'react'
import styled from '@emotion/styled'
import Logo from '@ui/assets/images/Logo.svg'
import WaveElement from '@ui/components/wave-element'
import TimeAndDate from '@ui/components/time-and-date'
import { useActiveTerminalTypes, useCmsStatus } from '@src/app/domain/global/stores'
import useNetworkStatus from '@src/app/domain/global/hooks/use-network-status'
import IconWifiOn from '@ui/components/icons/IconWifiOn'
import IconWifiOff from '@ui/components/icons/IconWifiOff'
import { useTranslation } from '@domain/administration/stores'
import ParcelLockerChoice from '@ui/assets/images/parcel-locker-choice.png'
import EntertainmentChoice from '@ui/assets/images/entertainment-choice.jpg'
import AtmChoice from '@ui/assets/images/atm-choice.png'
import AtmCasinoChoice from '@ui/assets/images/idle.jpg'
import { APP_ROUTERS, TERMINAL_TYPES } from '@src/app/domain/global/constants'

const ContainerHeader = styled.main`
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
  position: relative;
  height: 6.5rem;

  .left-side {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: 1.5rem;
    margin-top: -3rem;
    padding-left: 2rem;
  }

  .right-side {
    display: flex;
    flex-direction: row-reverse;
    align-items: center;
    gap: 2rem;
    padding: 1.5rem 2rem;
  }

  & .separator {
    width: 0.094rem;
    height: 2.875rem;
    background-color: var(--primary-medium);
  }
`

const Container = styled.main`
  display: flex;
  flex-direction: column;
  width: 100dvw;
  height: 100dvh;
`

const Header = styled.div`
  width: 100dvw;
  height: 14.375rem;
  background-color: var(--bg-medium);
  position: relative;

  .logo-container {
    position: absolute;
    top: 6.25rem;
    left: 3.25rem;
    background-color: var(--bg-medium);
    z-index: 5;

    .logo {
      width: 11.25rem;
      height: auto;
    }
  }

  .bg-image-container {
    margin-top: -3.5rem;
    position: relative;
    max-height: revert;
    overflow: visible;

    .bg-image {
      width: 100%;
      max-height: 25rem;
      object-fit: cover;
    }

    .wave {
      position: absolute;
      bottom: -3.125rem;
      height: auto;
      width: 100dvw;
    }

    .bg-img-logo-container {
      position: absolute;
      bottom: -7rem;
      left: 3.25rem;

      .bg-logo {
        width: 11.25rem;
        height: auto;
      }
    }
  }
`

const Main = styled.div`
  height: calc(100dvh - 14.375rem);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  flex-grow: 100;
  background-color: var(--bg-medium);
  padding: ${(p) => (p.padding ? '3.25rem 5.25rem' : '3.25rem 0 0 0')};

  & .main-content {
    flex-grow: 100;
    padding-bottom: 1.5rem;
    overflow: auto;

    .title-header {
      max-width: 40rem;
    }

    h1 {
      font-size: 4.375rem;
      line-height: 4.375rem;
      margin-left: 6.25rem;
      font-weight: 700;
    }

    & .experiences-wrapper {
      margin-top: 5rem;
      margin-left: 6.25rem;
      display: flex;
      gap: 1.5rem;
      overflow: auto;
      cursor: grab;

      scrollbar-width: none; /* Firefox */
      -ms-overflow-style: none; /* IE i Edge */

      &::-webkit-scrollbar {
        display: none; /* Chrome, Safari, Opera */
      }

      &.active {
        cursor: grabbing;
      }

      & .last {
        margin-right: 1rem;
      }
    }
  }
`

const ExperienceContainer = styled.div`
  display: flex;
  align-items: flex-end;
  padding: 2rem;
  border-radius: 1rem;
  flex: 0 0 auto;
  width: 25rem;
  height: 31.25rem;

  h3 {
    font-weight: 700;
    color: white;
    font-size: 3.438rem;
    line-height: 3.438rem;
  }

  background-image: ${(p) => `url(${p.image})`};
  background-size: cover;
  background-position: center;
  object-fit: fill;

  clip-path: path(
    'M174.852 0C193.759 2.20537e-05 211.734 8.19802 224.215 22.5059L229.522 28.5986C243.411 44.5152 263.065 55.0234 284.092 55.0234H292V55H384C392.837 55 400 62.1634 400 71V484C400 492.837 392.837 500 384 500H16C7.16345 500 4.10741e-07 492.837 0 484V16C1.41746e-06 7.16345 7.16345 2.17437e-07 16 0H174.852Z'
  );
`

const ExperienceSelector = ({ onChange = () => {} }) => {
  const cmsStatus = useCmsStatus()
  const { isOnline } = useNetworkStatus()
  const { t } = useTranslation()

  const [isDown, setIsDown] = useState(false)
  const [startX, setStartX] = useState(0)
  const [scrollLeft, setScrollLeft] = useState(0)
  const [isDragging, setIsDragging] = useState(false)
  const currentTerminalTypes = useActiveTerminalTypes()

  const wrapperRef = useRef(null)

  const handleMouseDown = (e) => {
    if (!wrapperRef.current) return
    setIsDown(true)
    setStartX(e.pageX - wrapperRef.current.offsetLeft)
    setScrollLeft(wrapperRef.current.scrollLeft)
    wrapperRef.current.classList.add('active')
    setIsDragging(false)
  }

  const handleMouseLeave = () => {
    setIsDown(false)
    if (wrapperRef.current) wrapperRef.current.classList.remove('active')
  }

  const handleMouseUp = () => {
    setIsDown(false)
    if (wrapperRef.current) wrapperRef.current.classList.remove('active')
  }

  const handleMouseMove = (e) => {
    if (!isDown || !wrapperRef.current) return
    e.preventDefault()
    const x = e.pageX - wrapperRef.current.offsetLeft
    const walk = (x - startX) * 2
    if (Math.abs(walk) > 5) setIsDragging(true)

    wrapperRef.current.scrollLeft = scrollLeft - walk
  }

  const renderConnectivityStatusIcon = () => {
    if (
      isOnline &&
      cmsStatus?.isCasinoManagementSystem === true &&
      cmsStatus?.isConnected === true
    ) {
      return <IconWifiOn color="green" size="m" />
    }

    if (isOnline) {
      return <IconWifiOn color="white" size="m" />
    }

    return <IconWifiOff color="red" size="m" />
  }

  return (
    <Container>
      <Header className="screen-header">
        <WaveElement position="bottom-left" bgImage={false} backgroundColor="var(--primary-dark)">
          <ContainerHeader>
            <div className="left-side">
              <div>{renderConnectivityStatusIcon()}</div>
            </div>
            <div className="right-side">
              <TimeAndDate />
            </div>
          </ContainerHeader>
        </WaveElement>

        <div className="logo-container">
          <img className="logo" src={Logo} />
        </div>
      </Header>

      <Main className="screen-content">
        <div className="main-content">
          <h1 className="title-header">{t('Please select your experience')}</h1>

          <div
            className="experiences-wrapper"
            ref={wrapperRef}
            onMouseDown={handleMouseDown}
            onMouseLeave={handleMouseLeave}
            onMouseUp={handleMouseUp}
            onMouseMove={handleMouseMove}
          >
            {currentTerminalTypes.includes(TERMINAL_TYPES.GAMING_ATM) && (
              <ExperienceContainer
                onClick={() => !isDragging && onChange(APP_ROUTERS.GAMING_ATM)}
                image={AtmCasinoChoice}
                className="experience-container "
              >
                <h3>
                  ATM
                  <br /> Casino
                </h3>
              </ExperienceContainer>
            )}
            {currentTerminalTypes.includes(TERMINAL_TYPES.PARCEL_LOCKER) && (
              <ExperienceContainer
                onClick={() => !isDragging && onChange(APP_ROUTERS.PARCEL_LOCKER)}
                image={ParcelLockerChoice}
                className="experience-container"
              >
                <h3>{t('Parcel Locker')}</h3>
              </ExperienceContainer>
            )}

            {currentTerminalTypes.includes(TERMINAL_TYPES.BANKING_ATM) && (
              <ExperienceContainer
                onClick={() => !isDragging && onChange(APP_ROUTERS.BANKING_ATM)}
                image={AtmChoice}
                className="experience-container"
              >
                <h3>ATM</h3>
              </ExperienceContainer>
            )}

            {currentTerminalTypes.includes(TERMINAL_TYPES.ENTERTAINMENT) && (
              <ExperienceContainer
                onClick={() => !isDragging && onChange(APP_ROUTERS.ENTERTAINMENT)}
                image={EntertainmentChoice}
                className="experience-container last"
              >
                <h3>Retail</h3>
              </ExperienceContainer>
            )}
          </div>
        </div>
      </Main>
    </Container>
  )
}

export default ExperienceSelector
