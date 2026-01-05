/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import Image from '@ui/assets/images/parcel-locker/take-package-v2.png'
import CircleButton from '@ui/components/circle-button'
import IconRedo from '@icons/IconRedo'
import { useTranslation } from '@src/app/domain/administration/stores'
import { useNavigate, useParams } from 'react-router-dom'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;
  position: relative;

  & .img {
    display: flex;
    flex-direction: row-reverse;
  }

  & .heading {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }
`

const TakePackageScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()
  const params = useParams()

  const { admin, lockerId } = params

  const handleAdminGoBack = () => {
    navigate(-1)
  }

  return (
    <ScreenContainerTop>
      <Container>
        <div className="img">
          <img src={Image} />
        </div>

        <div className="heading">
          <ScreenHeading
            middle={t('Take your parcel')}
            bottom={() => (
              <>
                {admin ? (
                  <span>
                    <span>{t('Please collect your parcel from')}</span>
                    <span>
                      <strong>{` ${t('Locker')} ${lockerId} `}</strong>
                    </span>
                    <span>{t('and close the locker')}</span>
                  </span>
                ) : (
                  <>{t('Please collect your parcel and close the locker.')}</>
                )}
              </>
            )}
          />

          {admin ? (
            <CircleButton
              size="l"
              color="medium"
              icon={(props) => <IconLeftHalfArrow {...props} />}
              textRight={t('Back')}
              onClick={() => handleAdminGoBack()}
              disabled={false}
            />
          ) : (
            <CircleButton
              size="l"
              color="medium"
              icon={(props) => <IconRedo {...props} />}
              textRight={t('Try again')}
              disabled={false}
              onClick={() => navigate('/parcel-locker/select-parcel-action')}
            />
          )}
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default TakePackageScreen
