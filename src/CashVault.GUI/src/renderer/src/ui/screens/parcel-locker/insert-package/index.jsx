/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import Image from '@ui/assets/images/parcel-locker/insert-package-v2.svg'
import CircleButton from '@ui/components/circle-button'
import IconRedo from '@icons/IconRedo'
import { useNavigate, useParams } from 'react-router-dom'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useTranslation } from '@src/app/domain/administration/stores'

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

  & .insert-actions {
    display: flex;
    gap: 4rem;
  }
`

const InsertPackageScreen = () => {
  const navigate = useNavigate()
  const params = useParams()

  const { admin, lockerId } = params

  const handleAdminGoBack = () => {
    navigate(-1)
  }

  const { t } = useTranslation()

  return (
    <ScreenContainerTop>
      <Container>
        <div className="img">
          <img src={Image} />
        </div>

        <div className="heading">
          <ScreenHeading
            middle={t('Insert parcel into open locker')}
            bottom={() => (
              <>
                {admin ? (
                  <span>
                    <span>{t('Please insert parcel into')}</span>
                    <span>
                      <strong>{` ${t('Locker')} ${lockerId} `}</strong>
                    </span>
                    <span>{t('and close the locker.')}</span>
                  </span>
                ) : (
                  t(
                    'Please insert parcel and close the locker. If your parcel doesn’t fit, you can change the size of the locker.'
                  )
                )}
              </>
            )}
          />

          <div className="insert-actions">
            {admin ? (
              <>
                <CircleButton
                  size="l"
                  color="medium"
                  icon={(props) => <IconLeftHalfArrow {...props} />}
                  textRight={t('Back')}
                  //onClick={() => handleAdminGoBack()}
                  onClick={() => navigate('/parcel-locker/shipments')}
                  disabled={false}
                />
                <CircleButton
                  size="l"
                  color="medium"
                  icon={(props) => <IconRedo {...props} />}
                  textRight={t('Scan another')}
                  disabled={false}
                  onClick={() => navigate('/parcel-locker/scan-parcel/barcode')}
                />
              </>
            ) : (
              <>
                <CircleButton
                  size="l"
                  color="medium"
                  icon={(props) => <IconRedo {...props} />}
                  textRight={t('Try again')}
                  disabled={false}
                  onClick={() =>
                    navigate(
                      '/parcel-locker/processing/send-open-locker/We are opening your locker...',
                      {
                        state: { retry: true }
                      }
                    )
                  }
                />
                <CircleButton
                  size="l"
                  color="dark"
                  icon={(props) => <IconRedo {...props} />}
                  textRight={t('Change locker size')}
                  disabled={false}
                />
              </>
            )}
          </div>
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default InsertPackageScreen
